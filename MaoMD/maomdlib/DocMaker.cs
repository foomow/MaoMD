﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

            foreach (var item in _assembly.GetReferencedAssemblies())
            {
                _logger.Information("Assembly:" + item.Name);
                Assembly.Load(item);
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
                    MakeTypeFile(type);
                    Content *= "### " + MakeTypeShowName(type);
                    Content *= MakeTypeTags(type);
                    Content *= MakeTypeInheritance(type);
                    Content *= MakeTypeAttributes(type);
                    Content *= ReadXmlDocTag(MakeNodeName(type), "summary");
                    if (type.IsNested)
                    {
                        Content *= "*nested in " + MakedName(type.DeclaringType, true) + "*";
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
                    MakeTypeFile(type);
                    Content *= "### " + MakeTypeShowName(type);
                    Content *= MakeTypeTags(type);
                    Content *= MakeTypeInheritance(type);
                    Content *= MakeTypeAttributes(type);
                    Content *= ReadXmlDocTag(MakeNodeName(type), "summary");
                    if (type.IsNested)
                    {
                        Content *= "*nested in " + MakedName(type.DeclaringType, true) + "*";
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
                    Content *= ReadXmlDocTag(MakeNodeName(type), "summary");
                    if (type.IsNested)
                    {
                        Content += "*nested in " + MakedName(type.DeclaringType, true) + "*";
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
                    MakeTypeFile(type);
                    Content *= "### " + MakeTypeShowName(type);
                    Content *= MakeTypeTags(type);
                    Content *= MakeTypeInheritance(type);
                    Content *= MakeTypeAttributes(type);
                    Content *= ReadXmlDocTag(MakeNodeName(type), "summary");
                    if (type.IsNested)
                    {
                        Content *= "*nested in " + MakedName(type.DeclaringType, true) + "*";
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
            Content *= "# " + MakeTypeShowName(type);
            string root = _linkRoot;
            if (root == "") root = Environment.CurrentDirectory;
            Content *= "Namespace: [" + type.Namespace + "](" + root + "/" + _outputDir + "/" + type.Namespace.Replace(".", "/") + "/home.md" + ")";
            Content *= MakeTypeTags(type);
            Content *= MakeTypeInheritance(type);
            Content *= "Usage: ";
            Content *= "## `" + MakeTypeUsage(type) + "`";
            Content *= MakeTypeAttributes(type);

            Content *= MakeArgumentsTable(type.GetTypeInfo());
            Content *= ReadXmlDocTag(MakeNodeName(type), "summary");
            Content *= ReadXmlDocTag(MakeNodeName(type), "remarks");
            Content *= ReadXmlDocTag(MakeNodeName(type), "seealso");

            MemberInfo[] members = type.GetMembers(
                BindingFlags.Instance
                | BindingFlags.Static
                | BindingFlags.Public
                | BindingFlags.NonPublic
                ).Where(x =>
                {
                    if (x.DeclaringType != type)
                        return false;
                    object[] attr = x.GetCustomAttributes(true).ToArray();
                    if (attr != null && attr.FirstOrDefault(y => y is System.Runtime.CompilerServices.CompilerGeneratedAttribute) != default(object))
                        return false;
                    return true;
                }).ToArray();
            MemberInfo[] types = members.Where(x => x.MemberType == MemberTypes.TypeInfo).ToArray();
            MemberInfo[] fields = members.Where(x => x.MemberType == MemberTypes.Field).ToArray();
            MemberInfo[] properties = members.Where(x => x.MemberType == MemberTypes.Property).ToArray();
            MemberInfo[] methods = members.Where(x => x.MemberType == MemberTypes.Method && x.MemberType != MemberTypes.Constructor).ToArray();
            MemberInfo[] constructors = members.Where(x => x.MemberType == MemberTypes.Constructor).ToArray();

            if (constructors.Length > 0)
            {
                Content *= "## Constructors:";
                Content += MakeMemberContent(constructors);
            }
            if (fields.Length > 0)
            {
                Content *= "## Fields:";
                Content += MakeMemberContent(fields);
            }
            if (properties.Length > 0)
            {
                Content *= "## Properties:";
                Content += MakeMemberContent(properties);
            }
            if (methods.Length > 0)
            {
                Content *= "## Methods:";
                Content += MakeMemberContent(methods);
            }


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
        private string MakedName(Type type, bool prefix = false, bool linked = true)
        {
            string ret = type.Name;
            if (type.GetGenericArguments().Length > 0)
            {
                ret = ret.Split("`")[0];
                ret += MakeGenericArgs(type);
            }
            if (prefix) ret = type.Namespace + "." + ret;
            if (type.Assembly == _assembly && !type.IsGenericMethodParameter && !type.IsGenericTypeParameter && linked)
            {
                ret = "[" + ret + "](" + MakeMDFilePath(type) + MakeMDFileName(type) + ")";
            }
            return ret;
        }
        private string MakeInheritanceString(Type type)
        {
            string ret = MakedName(type, true);
            if (type.BaseType != null)
            {
                string bret = MakeInheritanceString(type.BaseType);
                ret = bret + " -> " + ret;
            }
            return ret;
        }
        private string MakeParametersList(MethodBase info, bool link = true, bool paramName = true)
        {
            string Content = "";
            bool isExt = info.IsDefined(typeof(ExtensionAttribute), true);
            ParameterInfo[] parameters = info.GetParameters();
            foreach (ParameterInfo pinfo in parameters)
            {
                if (pinfo.Position > 0)
                    Content += ",";
                else
                if (isExt)
                    Content += "this ";
                if (pinfo.IsOut)
                    Content += "out ";
                if (pinfo.IsIn)
                    Content += "In ";

                string formatedName = "";
                if (link)
                    formatedName = MakedName(pinfo.ParameterType);
                else
                    formatedName = pinfo.ParameterType.FullName;

                if (pinfo.IsOut || pinfo.IsIn)
                {
                    formatedName=formatedName.TrimEnd('&');
                }

                if (formatedName.EndsWith('&'))
                {
                    formatedName="ref "+ formatedName.TrimEnd('&');
                }

                Content += formatedName;

                if (paramName)
                {
                    Content += " " + pinfo.Name;
                    if (pinfo.DefaultValue != null)
                    {
                        string valuestr = pinfo.DefaultValue.ToString();
                        if (valuestr != "")
                        {
                            if (pinfo.DefaultValue.GetType() == typeof(string) || pinfo.DefaultValue.GetType() == typeof(String))
                                valuestr = $"\"{valuestr}\"";
                            Content += " = " + valuestr;
                        }
                    }
                }
            }
            return Content;
        }
        private string MakeParametersListInNode(MethodBase info, bool link = true, bool paramName = true)
        {
            string Content = "";
            bool isExt = info.IsDefined(typeof(ExtensionAttribute), true);
            ParameterInfo[] parameters = info.GetParameters();
            foreach (ParameterInfo pinfo in parameters)
            {
                if (pinfo.Position > 0)
                    Content += ",";                

                string formatedName = "";
                if (link)
                    formatedName = MakedName(pinfo.ParameterType);
                else
                    formatedName = pinfo.ParameterType.FullName;


                formatedName = formatedName.Replace("&","@");

                Content += formatedName;

                if (paramName)
                {
                    Content += " " + pinfo.Name;
                    string valuestr = pinfo.DefaultValue.ToString();
                    if (valuestr != "")
                    {
                        if (pinfo.DefaultValue.GetType() == typeof(string) || pinfo.DefaultValue.GetType() == typeof(String))
                            valuestr = $"\"{valuestr}\"";
                        Content += " = " + valuestr;
                    }
                }
            }
            return Content;
        }
        private string MakeNodeName(object obj)
        {
            string ret = "";
            if (obj is Type)
            {
                ret = "T:" + ((Type)obj).FullName;
            }
            if (obj is MethodBase)
            {
                ret = "M:" + ((MethodBase)obj).DeclaringType.FullName;
                if (((MethodBase)obj).IsConstructor)
                    ret += ".#ctor";
                else
                    ret += "." + ((MethodBase)obj).Name;
                if (obj is MethodInfo)
                {
                    Type[] ptypes = ((MethodInfo)obj).GetGenericArguments();
                    if (ptypes != null && ptypes.Length > 0)
                        ret += "``" + ptypes.Length;
                }
                if (((MethodBase)obj).GetParameters().Length > 0)
                    ret += "(" + MakeParametersListInNode((MethodBase)obj, false, false) + ")";
            }
            if (obj is FieldInfo)
            {
                ret = "F:" + ((FieldInfo)obj).DeclaringType.FullName;
                ret += "." + ((FieldInfo)obj).Name;
            }
            if (obj is PropertyInfo)
            {
                ret = "P:" + ((PropertyInfo)obj).DeclaringType.FullName;
                ret += "." + ((PropertyInfo)obj).Name;
            }
            return ret;
        }
        private MarkDownContent ReadXmlDocTag(string nodename, string tag, string name = "")
        {
            MarkDownContent Content = new MarkDownContent();
            XmlNode node = _xmlnodes.SelectSingleNode("member[@name = \"" + nodename + "\"]");
            string title = tag.Substring(0, 1).ToUpper() + tag.Substring(1);
            if (name != "")
                tag = tag + "[@name=\"" + name + "\"]";
            if (node != null && node.SelectSingleNode(tag) != null)
            {


                if (tag == "seealso")
                {
                    Content += title + ": ";
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
                                    Content *= " - " + MakedName(satype);
                                else
                                    Content *= " - " + satype;
                            }
                        }
                    }
                }
                else
                {
                    if (title != "Typeparam" && title != "Param" && title != "Returns")
                    {
                        Content += title + ": ";
                        Content *= "> ";
                    }
                    Content += node.SelectSingleNode(tag).InnerText.Trim(' ').Trim('\t').Trim(' ').Trim('\t').Trim(Environment.NewLine.ToArray()).Trim(' ').Trim('\t').Trim(' ').Trim('\t');
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
            if (type.GetCustomAttributes(true).Length > 0)
            {
                Content = new MarkDownContent("Attributes: ");
            }
            bool begin = true;
            foreach (var attr in type.GetCustomAttributes(true))
            {
                if (!begin) Content += ", ";
                Content += MakedName(attr.GetType(), true);
                begin = false;
            }
            return Content;
        }
        private MarkDownContent MakeMemberAttributes(MemberInfo member)
        {
            MarkDownContent Content = new MarkDownContent();
            if (member.GetCustomAttributes(true).Length > 0)
            {
                Content = new MarkDownContent("Attributes: ");
            }
            bool begin = true;
            foreach (var attr in member.GetCustomAttributes(true))
            {
                if (!begin) Content += ", ";
                Content += MakedName(attr.GetType(), true);
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
            MarkDownContent Content = new MarkDownContent(MakedName(type));

            string tstr = "";
            if (type.IsSealed && type.IsAbstract) tstr = " static";
            if (type.IsAbstract && !type.IsInterface && tstr == "") Content += " abstract";
            if (type.IsSealed && !type.IsValueType && type.BaseType != typeof(MulticastDelegate) && tstr == "") Content += " sealed";

            Content += tstr;

            if (type.IsClass)
            {
                if (type.BaseType == typeof(MulticastDelegate))
                    Content += " Delegate";
                else
                    Content += " Class";
            }
            if (type.IsInterface) Content += " Interface";
            if (type.IsEnum) Content += " Enum";
            if (type.IsValueType && !type.IsEnum) Content += " Struct";
            return Content;
        }
        private MarkDownContent MakeTypeUsage(Type type)
        {
            MarkDownContent Content = new MarkDownContent();
            if (type.IsPublic) Content += "public ";
            string tstr = "";
            if (type.IsSealed && type.IsAbstract) tstr = "static ";
            if (type.IsAbstract && !type.IsInterface && tstr == "") Content += "abstract ";
            if (type.IsSealed && !type.IsValueType && type.BaseType != typeof(MulticastDelegate) && tstr == "") Content += "sealed ";

            Content += tstr;

            if (type.IsClass)
            {
                if (type.BaseType == typeof(MulticastDelegate))
                    Content += "delegate ";
                else
                    Content += "class ";
            }
            if (type.IsInterface) Content += "interface ";
            if (type.IsEnum) Content += "enum ";
            if (type.IsValueType && !type.IsEnum) Content += "struct ";

            if (type.GetGenericArguments().Length > 0)
            {
                string formatedName = type.Name;
                formatedName = formatedName.Split("`")[0];
                Content += formatedName + MakeGenericArgs(type);
            }
            else
            {
                Content += type.Name;
            }
            string appendstr = "";
            if (type.IsEnum)
                appendstr = "";
            if (type.BaseType != null && type.BaseType.Assembly == _assembly)
            {
                if (appendstr == "") appendstr += " :";
                appendstr += type.BaseType.Name;
            }
            Type[] intfs = type.GetInterfaces();
            foreach (Type intf in intfs)
            {
                if (appendstr == "") appendstr += " :";
                else
                    appendstr += ", ";
                appendstr += intf.Name;
            }

            if (appendstr != "") Content += appendstr;
            return Content;
        }
        private MarkDownContent MakeMemberUsage(MemberInfo memberInfo)
        {
            MarkDownContent Content = new MarkDownContent();
            if (memberInfo.MemberType == MemberTypes.Constructor)
            {
                ConstructorInfo info = (ConstructorInfo)memberInfo;
                Content += info.DeclaringType.Name.Split("`")[0];
                Content += " ( ";
                Content += MakeParametersList(info);
                Content += " ); ";
            }
            if (memberInfo.MemberType == MemberTypes.Method)
            {
                MethodInfo info = (MethodInfo)memberInfo;
                if (!info.IsSpecialName)
                {
                    if (info.IsPublic) Content += "public ";
                    if (info.IsPrivate) Content += "private ";
                    if (info.IsAssembly) Content += "internal ";
                    if (info.IsFamily) Content += "protected ";

                    if (info.IsVirtual && info.GetBaseDefinition() == info) Content += "virtual ";
                    if (info.GetBaseDefinition() != info) Content += "override ";
                    if (info.IsFinal) Content += "final ";
                    if (info.IsAbstract) Content += "abstract ";

                    if (info.IsStatic) Content += "static ";

                    if (info.ReturnType != null) Content += MakedName(info.ReturnType) + " ";

                    Content += info.Name;
                    Content += MakeGenericArgs(info);

                    Content += " ( ";
                    Content += MakeParametersList(info);
                    Content += " ); ";
                }
            }
            if (memberInfo.MemberType == MemberTypes.Field)
            {
                FieldInfo info = (FieldInfo)memberInfo;
                if (!info.IsSpecialName)
                {
                    if (info.IsPublic) Content += "public ";
                    if (info.IsPrivate) Content += "private ";
                    if (info.IsAssembly) Content += "internal ";
                    if (info.IsFamily) Content += "protected ";
                    if (info.IsInitOnly) Content += "readonly ";
                    if (info.IsLiteral) Content += "const ";

                    if (info.IsStatic) Content += "static ";

                    Content += MakedName(info.FieldType) + " ";

                    Content += info.Name;
                }
            }
            if (memberInfo.MemberType == MemberTypes.Property)
            {
                PropertyInfo info = (PropertyInfo)memberInfo;
                if (!info.IsSpecialName)
                {
                    Content += MakedName(info.PropertyType) + " ";
                    Content += info.Name;
                    Content += "{ ";
                    MethodInfo[] accessors = info.GetAccessors();
                    foreach (MethodInfo method in accessors)
                    {
                        Content += method.Name.Substring(0, 3) + "; ";
                    }
                    Content += "}";
                }
            }
            return Content;
        }
        private string MakeGenericArgs(MethodInfo info)
        {
            string Content = "";
            Type[] ptypes = info.GetGenericArguments();
            if (ptypes.Length > 0)
            {
                Content += "< ";
                bool begin = true;
                foreach (Type ptype in ptypes)
                {
                    if (!begin) Content += ", ";
                    Content += MakedName(ptype);
                    begin = false;
                }
                Content += " >";
            }
            return Content;
        }
        private string MakeGenericArgs(Type info)
        {
            string Content = "";
            Type[] ptypes = info.GetGenericArguments();
            if (ptypes.Length > 0)
            {
                Content += "< ";
                bool begin = true;
                foreach (Type ptype in ptypes)
                {
                    if (!begin) Content += ", ";
                    Content += MakedName(ptype);
                    begin = false;
                }
                Content += " >";
            }
            return Content;
        }
        private MarkDownContent MakeMemberContent(MemberInfo[] members)
        {
            MarkDownContent Content = new MarkDownContent();
            foreach (MemberInfo info in members)
            {
                MarkDownContent membercontent = MakeMemberUsage(info);
                if (membercontent != "")
                {
                    Content *= "### " + MakeMemberUsage(info);
                    Content *= MakeMemberAttributes(info);
                    Content *= ReadXmlDocTag(MakeNodeName(info), "summary");
                    if (info is MethodInfo || info is ConstructorInfo)
                    {
                        Content *= MakeParamsTable((MethodBase)info);
                        if (info is MethodInfo)
                            Content *= "Returns: " + ReadXmlDocTag(MakeNodeName(info), "returns");
                    }
                    Content *= ReadXmlDocTag(MakeNodeName(info), "remarks");
                    Content *= ReadXmlDocTag(MakeNodeName(info), "seealso");
                    Content *= "---";
                }
            }
            return Content;
        }
        private MarkDownContent MakeParamsTable(MethodBase method)
        {
            MarkDownContent Content = new MarkDownContent();
            if (method.GetParameters().Length > 0)
            {
                Content += "Parameters:";
                Content *= "| Name | Type | Summary |";
                Content /= "| --- | --- | --- |";
                foreach (ParameterInfo pinfo in method.GetParameters())
                {
                    string desc = "*无*";
                    string summary = ReadXmlDocTag(MakeNodeName(method), "param", pinfo.Name);
                    if (summary != "") desc = summary;
                    Content /= "| " + pinfo.Name + " | " + MakedName(pinfo.ParameterType) + " | " + desc + " |";
                }
            }
            if (method is MethodInfo && ((MethodInfo)method).GetGenericArguments().Length > 0)
            {
                if (Content != "") Content++;
                Content += "Arguments:";
                Content *= "| Name | Summary |";
                Content /= "| --- | --- |";
                foreach (Type pinfo in ((MethodInfo)method).GetGenericArguments())
                {
                    string desc = "*无*";
                    string summary = ReadXmlDocTag(MakeNodeName(method), "typeparam", pinfo.Name);
                    if (summary != "") desc = summary;
                    Content /= "| " + pinfo.Name + " | " + desc + " |";
                }
            }
            return Content;
        }
        private MarkDownContent MakeArgumentsTable(TypeInfo ctor)
        {
            MarkDownContent Content = new MarkDownContent();
            if (ctor.GetGenericArguments().Length > 0)
            {
                if (Content != "") Content++;
                Content += "Arguments:";
                Content *= "| Name | Summary |";
                Content /= "| --- | --- |";
                foreach (Type pinfo in ctor.GetGenericArguments())
                {
                    string desc = "*无*";
                    string summary = ReadXmlDocTag(MakeNodeName(ctor), "typeparam", pinfo.Name);
                    if (summary != "") desc = summary;
                    Content /= "| " + pinfo.Name + " | " + desc + " |";
                }
            }
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
