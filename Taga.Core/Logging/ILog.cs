using System;

namespace Taga.Core.Logging
{
    public interface ILog
    {
        DateTime Date { get; }
        LogLevel Level { get; }
        string ErrorCode { get; }
        string Message { get; }
        string User { get; }
        string Details { get; }
    }
}
