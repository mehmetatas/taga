using System;
using Taga.Framework.Exceptions;

namespace Taga.Framework.Logging.Impl
{
    public class Log : ILog
    {
        public DateTime Date { get; set; }
        public LogLevel Level { get; set; }
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string User { get; set; }
        public string Details { get; set; }

        public static Log Exception(Exception ex)
        {
            return new Log
            {
                Level = LogLevel.Error,
                Date = DateTime.Now,
                Details = ex.ToString(),
                ErrorCode = (ex as Error ?? Errors.Unknown).MessageCode,
                Message = ex.Message
            };
        }
    }
}
