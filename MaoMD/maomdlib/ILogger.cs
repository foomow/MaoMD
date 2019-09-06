using System;
using System.Collections.Generic;
using System.Text;

namespace maomdlib
{
    public interface ILogger:IDisposable
    {
        void Trace(string format, params object[] args);
        void Debug(string format, params object[] args);
        void Information(string format, params object[] args);
        void Warning(string format, params object[] args);
        void Error(string format, params object[] args);
        void Critical(string format, params object[] args);        
        LogLevel MinLogLevel { get; set; }
    }
}
