using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Taga.Framework.Logging.Impl
{
    public class SimpleLogger : ILogger
    {
        private readonly string _path;
        private readonly Queue<ILog> _queue;

        public SimpleLogger()
        {
            _path = ConfigurationManager.AppSettings["logpath"];
            _queue = new Queue<ILog>();
            Task.Factory.StartNew(WriteLogs);
        }

        private void WriteLogs()
        {
            var sb = new StringBuilder();

            while (true)
            {
                try
                {     
                    lock (_queue)
                    {
                        while (_queue.Count > 0)
                        {
                            var log = _queue.Dequeue();
                            sb.AppendFormat($"{log.Date.ToString("s")} {log.Level.ToString().ToUpperInvariant().PadRight(10)}: {log.ErrorCode} -> {log.Message}");
                            sb.AppendLine();
                            sb.Append(log.Details);
                        }
                    }

                    if (sb.Length == 0)
                    {
                        continue;
                    }

                    using (var sw = new StreamWriter(_path, true))
                    {
                        sw.WriteLine(sb.ToString());
                    }

                    sb.Clear();
                }
                catch
                {

                }
                finally
                {
                    Thread.Sleep(10000);
                }
            }
        }

        public void Log(ILog log)
        {
            lock (_queue)
            {
                _queue.Enqueue(log);
            }
        }
    }
}
