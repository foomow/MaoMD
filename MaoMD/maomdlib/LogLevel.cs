using System;
using System.Collections.Generic;
using System.Text;

namespace maomdlib
{
    /// <summary>
    /// the enum for log level
    /// </summary>
    public enum LogLevel: byte
    {
        /// <summary>
        /// trace
        /// </summary>
        Trace,
        /// <summary>
        /// debug
        /// </summary>
        Debug,
        /// <summary>
        /// information
        /// </summary>
        Information,
        /// <summary>
        /// warning
        /// </summary>
        Warning,
        /// <summary>
        /// error
        /// </summary>
        Error,
        /// <summary>
        /// critical
        /// </summary>
        Critical
    }
}
