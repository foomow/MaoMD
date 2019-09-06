using System;
using System.IO;

namespace maomdlib
{
    public class DocMaker : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _dllFile;
        private readonly string _xmlFile;
        private readonly string _outputDir;
        public DocMaker(string dllFile = "", string xmlFile = "", string outputDir = "", ILogger logger = null)
        {
            _logger = logger == null ? new ConsoleLogger() : logger;

            while (dllFile.EndsWith(".dll")) dllFile = dllFile.Remove(dllFile.LastIndexOf(".dll"));
            while (xmlFile.EndsWith(".xml")) xmlFile = xmlFile.Remove(xmlFile.LastIndexOf(".xml"));
            if (xmlFile == "") xmlFile = dllFile;
            if (outputDir == "") outputDir = dllFile;

            _dllFile = dllFile;
            _xmlFile = xmlFile;
            _outputDir = outputDir;
        }

        public void Make()
        {
            if (!CheckFiles()) {
                _logger.Information("文档生成失败。");
            };
            _logger.Trace("xixi");
            _logger.Information("what?? ");
            _logger.Debug("haha");
            _logger.Warning("haha{0}", "!!");
            _logger.Error("xixi");
            _logger.Critical("what?? ");
        }

        private bool CheckFiles()
        {
            if (!File.Exists(_dllFile + ".dll"))
            {
                _logger.Error("dll 文件不存在");
                return false;
            }
            if (!File.Exists(_xmlFile + ".xml"))
            {
                _logger.Error("xml 文件不存在");
                return false;
            }
            return true;
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
