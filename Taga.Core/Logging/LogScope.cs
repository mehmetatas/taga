using System;
using System.Collections.Generic;
using System.Linq;
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
        }

        public void Log(ILog log)
        {
            _logs.Add(log);
        }

        public void Flush(LogLevel minLogLevel)
        {
            var logger = ServiceProvider.Provider.GetOrCreate<ILogger>();
            foreach (var log in _logs.Where(l => l.Level >= minLogLevel))
            {
                logger.Log(log);
            }
            _logs.Clear();
        }

        public void Dispose()
        {
            Flush(LogLevel.Error);
        }
    }
}
