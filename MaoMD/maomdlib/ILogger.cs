using System;
using System.Collections.Generic;
using System.Text;

namespace maomdlib
{
    /// <summary>
    /// the interface of logger
    /// </summary>
    public interface ILogger:IDisposable
    {
        /// <summary>
        /// log logic for this log level
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="args">args</param>
        void Trace(string format, params object[] args);
        /// <summary>
        /// log logic for this log level
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="args">args</param>
        void Debug(string format, params object[] args);
        /// <summary>
        /// log logic for this log level
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="args">args</param>
        void Information(string format, params object[] args);
        /// <summary>
        /// log logic for this log level
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="args">args</param>
        void Warning(string format, params object[] args);
        /// <summary>
        /// log logic for this log level
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="args">args</param>
        void Error(string format, params object[] args);
        /// <summary>
        /// log logic for this log level
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="args">args</param>
        void Critical(string format, params object[] args); 
        /// <summary>
        /// the minimize level to log
        /// </summary>
        LogLevel MinLogLevel { get; set; }
    }
}
