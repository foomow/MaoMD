using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace maomdlib
{
    /// <summary>
    /// a logger for console
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        /// <summary>
        /// a struct for messages
        /// </summary>
        private struct MESSAGE
        {
            public LogLevel level;
            public string message;
        }
        /// <summary>
        /// message queue
        /// </summary>
        private readonly Queue<MESSAGE> _messagePool;
        /// <summary>
        /// exit trigger
        /// </summary>
        private bool _exit;
        /// <summary>
        /// the thread for logging
        /// </summary>
        private Thread _logThread;
        /// <summary>
        /// the minimize level for logging
        /// </summary>
        private LogLevel _minLogLevel;
        /// <summary>
        /// the minimize level for logging
        /// </summary>
        public LogLevel MinLogLevel { get => _minLogLevel; set => _minLogLevel = value; }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="minLogLevel">the minimize level for logging</param>
        public ConsoleLogger(LogLevel minLogLevel = LogLevel.Trace)
        {
            _exit = false;
            _minLogLevel = minLogLevel;
            _messagePool = new Queue<MESSAGE>();
            _logThread = new Thread(new ThreadStart(LogProcess));
            _logThread.Start();
        }
        /// <summary>
        /// the thread for logging
        /// </summary>
        private void LogProcess()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "][Logger] Start");
            Console.ResetColor();
            while (!_exit || _messagePool.Count > 0)
            {
                lock (_messagePool)
                {
                    if (_messagePool.Count > 0)
                    {
                        MESSAGE message = _messagePool.Dequeue();
                        Console.ForegroundColor = ConsoleColor.White;
                        switch (message.level)
                        {
                            case LogLevel.Trace:
                                Console.ForegroundColor = ConsoleColor.Gray;
                                break;
                            case LogLevel.Debug:
                                Console.ForegroundColor = ConsoleColor.White;
                                break;
                            case LogLevel.Information:
                                Console.ForegroundColor = ConsoleColor.Green;
                                break;
                            case LogLevel.Warning:
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                break;
                            case LogLevel.Error:
                                Console.ForegroundColor = ConsoleColor.Red;
                                break;
                            case LogLevel.Critical:
                                Console.ForegroundColor = ConsoleColor.DarkRed;
                                break;
                            default:
                                break;
                        }
                        Console.WriteLine(message.message);
                        Console.ResetColor();

                    }
                }
                Thread.Sleep(10);
            }
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ffff") + "][Logger] Stop");
            Console.ResetColor();
        }
        /// <summary>
        /// log logic for critical
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="args">args</param>
        public void Critical(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            AddLog(LogLevel.Critical, msg);
        }
        /// <summary>
        /// log logic for Debug
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="args">args</param>
        public void Debug(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            AddLog(LogLevel.Debug, msg);
        }
        /// <summary>
        /// log logic for this log level
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="args">args</param>
        public void Error(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            AddLog(LogLevel.Error, msg);
        }
        /// <summary>
        /// log logic for this log level
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="args">args</param>
        public void Information(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            AddLog(LogLevel.Information, msg);
        }
        /// <summary>
        /// log logic for this log level
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="args">args</param>
        public void Trace(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            AddLog(LogLevel.Trace, msg);
        }
        /// <summary>
        /// log logic for this log level
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="args">args</param>
        public void Warning(string format, params object[] args)
        {
            string msg = string.Format(format, args);
            AddLog(LogLevel.Warning, msg);
        }
       /// <summary>
       /// add log to queue
       /// </summary>
       /// <param name="level">the level</param>
       /// <param name="msg">message</param>
        private void AddLog(LogLevel level, string msg)
        {
            string date = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff");
            MESSAGE message = new MESSAGE()
            {
                level = level,
                message = $"[{date}]" + $"[{level.ToString()}] " + msg
            };
            lock (_messagePool)
            {
                _messagePool.Enqueue(message);
            }
        }

        #region IDisposable Support
        /// <summary>
        /// To detect redundant calls
        /// </summary>
        private bool disposedValue = false; // To detect redundant calls
        /// <summary>
        /// dispose method
        /// </summary>
        /// <param name="disposing">redundant detector</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _exit = true;
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Logger()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// deconstructor
        /// </summary>
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
