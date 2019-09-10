using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace maomdlib
{
    public class DocMaker : IDisposable
    {
        private readonly bool _initiallized;
        private readonly ILogger _logger;
        private readonly string _dllFile;
        private readonly string _xmlFile;
        private readonly string _outputDir;
        private readonly string _linkRoot;
        private readonly Assembly _assembly;
        private readonly XmlNode _xmlnodes;
        public DocMaker(string dllFile = "", string xmlFile = "", string outputDir = "", string linkRoot = "", ILogger logger = null)
        {
            _initiallized = false;
            _logger = logger ?? new ConsoleLogger();

            while (dllFile.EndsWith(".dll")) dllFile = dllFile.Remove(dllFile.LastIndexOf(".dll"));
            while (xmlFile.EndsWith(".xml")) xmlFile = xmlFile.Remove(xmlFile.LastIndexOf(".xml"));
            if (xmlFile == "") xmlFile = dllFile;
            if (outputDir == "") outputDir = dllFile;

            _dllFile = dllFile;
            _xmlFile = xmlFile;
            _outputDir = outputDir;
            _linkRoot = linkRoot;

            if (!CheckFiles())
            {
                _logger.Information("文档生成失败。");
                return;
            };

            if (!Directory.Exists(_outputDir))
            {
                _logger.Information("创建目标目录");
                Directory.CreateDirectory(_outputDir);
            }
            else
            {
                _logger.Information("清空目标目录");
                if (CleanDir(_outputDir))
                    _logger.Information("清空目录 {0} 成功！", _outputDir);
                else
                {
                    _logger.Information("清空目录 {0} 失败！", _outputDir);
                    return;
                }
            }

            _logger.Information("加载 dll 文件:{0}.dll", _dllFile);
            try
            {
                _assembly = Assembly.LoadFrom(_dllFile + ".dll");
                _logger.Information("加载 dll 文件成功:{0}.dll", _dllFile);
            }
            catch (Exception e)
            {
                _logger.Error("加载 dll 文件失败:{0}.dll\r\n{1}", _dllFile, e.Message);
                return;
            }

            _logger.Information("加载 xml 文件:{0}.xml", _xmlFile);
            try
            {
                XmlDocument XmlDoc = new XmlDocument();
                XmlDoc.Load(_xmlFile + ".xml");
                _xmlnodes = XmlDoc.SelectSingleNode("doc/members");
                _logger.Information("加载 xml 文件成功:{0}.xml", _xmlFile);
            }
            catch (Exception e)
            {
                _logger.Error("加载 xml 文件失败:{0}.xml\r\n{1}", _xmlFile, e.Message);
                return;
            }

            _logger.Information("DocMaker 创建成功！");
            _initiallized = true;
        }
        public MakeResult Make()
        {
            if (_initiallized)
            {
                MakeRootHome();
                _logger.Information("执行成功！");
                return new MakeResult(0, "成功");
            }
            else
            {
                _logger.Information("失败，DocMaker 对象初始化异常！");
                this.Dispose(true);
                return new MakeResult(-1, "失败");
            }

        }
        private bool MakeRootHome()
        {
            MarkDownContent Content;
            string[] NameInfo = _assembly.FullName.Split(",");
            Content = new MarkDownContent("# " + NameInfo[0] + " Project");
            Content *= "Version: " + NameInfo[1];
            Content *= "Culture: " + NameInfo[2];
            Content *= "PublicKeyToken: " + NameInfo[3];

            string[] NameSpaces = _assembly.GetTypes().Select(x => x.Namespace).Distinct().OrderBy(x => x).ToArray();

            Content *= "| Namespace | 说明 |";
            Content /= "| --- | --- |";
            foreach (string ns in NameSpaces)
            {
                if (ns != null)
                {
                    Content /= "| [" + ns + "](" + MakeNameSpaceHome(ns) + ") | **请补充** |";
                }
            }

            string filename = @"\home.md";
            try
            {
                File.WriteAllText(_outputDir + filename, Content);
            }
            catch (Exception e)
            {
                _logger.Error("文件写入失败:{0}\r\n{1}", filename, e.Message);
                return false;
            }
            return true;
        }
        private string MakeNameSpaceHome(string ns)
        {
            string root = _linkRoot;
            if (root == "") root = Environment.CurrentDirectory;
            MarkDownContent Content = new MarkDownContent("# " + ns + " Namespace");

            Content *= "Description: *TODO*";

            Type[] types = _assembly.GetTypes().Where(x => x.Namespace == ns).ToArray();

            Type[] structs = types.Where(x => x.IsValueType && !x.IsEnum).ToArray();
            if (structs.Length > 0)
            {
                Content *= "---";
                Content *= "## Structs:";
                foreach (Type type in structs)
                {
                    Content *= "### " + MakeTypeShowName(type);
                    Content *= MakeTypeTags(type);
                    Content *= MakeTypeInheritance(type);
                    Content *= MakeTypeAttributes(type);
                    Content *= ReadXmlDocTag(type, "summary");
                    if (type.IsNested)
                    {
                        Content *= "*nested in " + MakeLinkedName(type.DeclaringType, true) + "*";
                    }
                }
            }

            Type[] interfaces = types.Where(x => x.IsInterface).ToArray();
            if (interfaces.Length > 0)
            {
                Content *= "---";
                Content *= "## Interfaces:";
                foreach (Type type in interfaces)
                {
                    Content *= "### " + MakeTypeShowName(type);
                    Content *= MakeTypeTags(type);
                    Content *= MakeTypeInheritance(type);
                    Content *= MakeTypeAttributes(type);
                    Content *= ReadXmlDocTag(type, "summary");
                    if (type.IsNested)
                    {
                        Content *= "*nested in " + MakeLinkedName(type.DeclaringType, true) + "*";
                    }
                }
            }

            Type[] classes = types.Where(x => x.IsClass).ToArray();
            if (classes.Length > 0)
            {
                Content *= "---";
                Content *= "## Classes:";
                foreach (Type type in classes)
                {
                    MakeTypeFile(type);
                    Content *= "### " + MakeTypeShowName(type);
                    Content *= MakeTypeTags(type);
                    Content *= MakeTypeInheritance(type);
                    Content *= MakeTypeAttributes(type);
                    Content *= ReadXmlDocTag(type, "summary");
                    if (type.IsNested)
                    {
                        Content += "*nested in " + MakeLinkedName(type.DeclaringType, true) + "*";
                    }
                }
            }

            Type[] enums = types.Where(x => x.IsEnum).ToArray();
            if (enums.Length > 0)
            {
                Content *= "---";
                Content *= "## Enums:";
                foreach (Type type in enums)
                {
                    Content *= "### " + MakeTypeShowName(type);
                    Content *= MakeTypeTags(type);
                    Content *= MakeTypeInheritance(type);
                    Content *= MakeTypeAttributes(type);
                    Content *= ReadXmlDocTag(type, "summary");
                    if (type.IsNested)
                    {
                        Content *= "*nested in " + MakeLinkedName(type.DeclaringType, true) + "*";
                    }
                }
            }

            string pathname = "/" + ns.Replace(".", "/");
            string filename = pathname + @"\home.md";
            string link;
            try
            {
                if (!Directory.Exists(_outputDir + pathname)) Directory.CreateDirectory(_outputDir + pathname);
                File.WriteAllText(_outputDir + filename, Content);
                link = root + "/" + _outputDir + filename;
            }
            catch (Exception e)
            {
                _logger.Error("文件写入失败:{0}\r\n{1}", filename, e.Message);
                return "";
            }
            return link;
        }
        private void MakeTypeFile(Type type)
        {
            MarkDownContent Content = new MarkDownContent();
            Content *= "### " + MakeTypeShowName(type);
            string root = _linkRoot;
            if (root == "") root = Environment.CurrentDirectory;
            Content *= "Namespace: [" + type.Namespace + "](" + root + "/" + _outputDir + "/" + type.Namespace.Replace(".", "/") + "/home.md" + ")";
            Content *= MakeTypeTags(type);
            Content *= MakeTypeInheritance(type);
            Content *= MakeTypeAttributes(type);
            Content *= ReadXmlDocTag(type, "summary");
            Content *= ReadXmlDocTag(type, "remarks");
            Content *= ReadXmlDocTag(type, "seealso");

            MemberInfo[] members = type.GetMembers();
            MemberInfo[] types = members.Where(x => x.MemberType == MemberTypes.TypeInfo).ToArray();
            MemberInfo[] fields = members.Where(x => x.MemberType == MemberTypes.Field).ToArray();
            MemberInfo[] properties = members.Where(x => x.MemberType == MemberTypes.Property).ToArray();
            MemberInfo[] methods = members.Where(x => x.MemberType == MemberTypes.Method).ToArray();
            MemberInfo[] nestedtypes = members.Where(x => x.MemberType == MemberTypes.NestedType).ToArray();
            MemberInfo[] constructors = members.Where(x => x.MemberType == MemberTypes.Constructor).ToArray();
            MemberInfo[] events = members.Where(x => x.MemberType == MemberTypes.Event).ToArray();

            string pathname = "/" + type.Namespace.Replace(".", "/");
            string filename = pathname + "//" + MakeMDFileName(type);
            try
            {
                if (!Directory.Exists(_outputDir + pathname)) Directory.CreateDirectory(_outputDir + pathname);
                File.WriteAllText(_outputDir + filename, Content);
            }
            catch (Exception e)
            {
                _logger.Error("文件写入失败:{0}\r\n{1}", filename, e.Message);
            }
        }
        private string MakeMDFilePath(Type type)
        {
            string root = _linkRoot;
            if (root == "") root = Environment.CurrentDirectory;
            return root + "/" + _outputDir + "/" + type.Namespace.Replace(".", "/") + "/";
        }
        private string MakeMDFileName(Type type)
        {
            string name = type.Name;
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(name);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                name = sb.ToString() + ".md";
            }
            return name;
        }
        private string MakeLinkedName(Type type, bool prefix = false)
        {
            string ret = type.Name;
            if (prefix) ret = type.Namespace + "." + ret;
            if (type.Assembly == _assembly)
            {
                ret = "[" + ret + "](" + MakeMDFilePath(type) + MakeMDFileName(type) + ")";
            }
            return ret;
        }
        private string MakeInheritanceString(Type type)
        {
            string ret = MakeLinkedName(type, true);
            if (type.BaseType != null)
            {
                string bret = MakeInheritanceString(type.BaseType);
                ret = bret + " -> " + ret;
            }
            return ret;
        }
        private MarkDownContent ReadXmlDocTag(Type type, string tag)
        {
            MarkDownContent Content = new MarkDownContent();
            string typeChsr = "T";
            XmlNode node = _xmlnodes.SelectSingleNode("member[@name = \"" + typeChsr + ":" + type.FullName + "\"]");
            if (node != null && node.SelectSingleNode(tag) != null)
            {
                Content += tag + ": ";

                if (tag == "seealso")
                {
                    XmlNodeList nodes = node.SelectNodes(tag);
                    foreach (XmlNode n in nodes)
                    {
                        if (n.Attributes["cref"] != null)
                        {
                            string mtype = n.Attributes["cref"].Value.Substring(0, 1);
                            string mname = n.Attributes["cref"].Value.Substring(2);
                            if (mtype == "T")
                            {
                                Type satype = _assembly.GetType(mname);
                                if (satype != null)
                                    Content *= " - " + MakeLinkedName(satype);
                                else
                                    Content *= " - " + satype;
                            }
                        }
                    }
                }
                else
                {
                    Content *= ">";
                    Content += node.SelectSingleNode(tag).InnerText;
                }
            }
            return Content;
        }
        private MarkDownContent MakeTypeInheritance(Type type)
        {
            MarkDownContent Content = new MarkDownContent("Inheritance: ");
            if (type.BaseType != null)
                Content += MakeInheritanceString(type);
            return Content;
        }
        private MarkDownContent MakeTypeAttributes(Type type)
        {
            MarkDownContent Content = new MarkDownContent();
            if (type.CustomAttributes.Count() > 0)
            {
                Content = new MarkDownContent("Attributes: ");
            }
            bool begin = true;
            foreach (var attr in type.GetCustomAttributes(true))
            {
                if (!begin) Content += ", ";
                Content += MakeLinkedName(attr.GetType(), true);
                begin = false;
            }
            return Content;
        }
        private MarkDownContent MakeTypeTags(Type type)
        {
            MarkDownContent Content = new MarkDownContent("Tags: ");

            foreach (PropertyInfo info in typeof(Type).GetProperties())
            {
                if (info.PropertyType == typeof(bool))
                {
                    Content += (bool)info.GetValue(type) ? " `" + info.Name.Substring(2) + "`" : "";
                }
            }

            return Content;
        }
        private MarkDownContent MakeTypeShowName(Type type)
        {
            MarkDownContent Content = new MarkDownContent();
            if (type.IsSealed && !type.IsValueType) Content += "sealed";
            if (type.IsAbstract && !type.IsInterface) Content += "abstract";
            if (type.IsPublic) Content += " public";
            if (type.IsClass) Content += " class";
            if (type.IsInterface) Content += " interface";
            if (type.IsEnum) Content += " enum";

            Content += " " + MakeLinkedName(type);
            return Content;
        }
        private bool CheckFiles()
        {
            if (!File.Exists(_dllFile + ".dll"))
            {
                _logger.Error(_dllFile + ".dll 文件不存在");
                return false;
            }
            if (!File.Exists(_xmlFile + ".xml"))
            {
                _logger.Error("xml 文件不存在");
                return false;
            }
            return true;
        }
        private bool CleanDir(string path)
        {
            if (Directory.Exists(path))
            {
                string[] files = Directory.GetFiles(path);
                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception e)
                    {
                        _logger.Error("删除文件 {0} 时出错：{1}", file, e.Message);
                        return false;
                    }
                }
                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                {
                    if (CleanDir(directory))
                    {
                        try
                        {
                            Directory.Delete(directory);
                        }
                        catch (Exception e)
                        {
                            _logger.Error("删除目录 {0} 时出错：{1}", directory, e.Message);
                            return false;
                        }
                    }
                }
                return true;
            }
            return false;
        }
        public struct MakeResult
        {
            public int code;
            public string message;

            public MakeResult(int code = 0, string message = "")
            {
                this.code = code;
                this.message = message;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _logger.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DocMaker()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        void IDisposable.Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
