using System;
using System.Collections.Generic;
using Taga.Core.Context;
using Taga.Core.IoC;

namespace Taga.Core.Logging
{
    public class LogScope : IDisposable
    {
        public static LogScope Current
        {
            get
            {
                var scope = CallContext.Current["Taga.Core.Logging.LogScope"] as LogScope;
                if (scope == null)
                {
                    scope = new LogScope();
                    CallContext.Current["Taga.Core.Logging.LogScope"] = scope;
                }
                return scope;
            }
        }

        private readonly List<ILog> _logs;

        private LogScope()
        {
            _logs = new List<ILog>();
            MaxLevel = LogLevel.Debug;
        }

        public LogLevel MaxLevel { get; private set; }

        public void Log(ILog log)
        {
            _logs.Add(log);

            if (log.Level > MaxLevel)
            {
                MaxLevel = log.Level;
            }
        }

        public void Flush(LogLevel minLogLevel, LogLevel treshhold)
        {
            if (MaxLevel >= treshhold)
            {
                Flush(minLogLevel);
            }
        }

        public void Flush(LogLevel minLogLevel)
        {
            var logger = ServiceProvider.Provider.GetOrCreate<ILogger>();
            lock (_logs)
            {
                while (_logs.Count > 0)
                {
                    var log = _logs[0];
                    if (log.Level >= minLogLevel)
                    {
                        logger.Log(log);
                    }
                    _logs.RemoveAt(0);
                }
            }
        }

        public void Dispose()
        {
            Flush(LogLevel.Error);
        }
    }
}
