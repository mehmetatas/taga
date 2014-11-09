using System;

namespace Taga.Core.Logging
{
    public interface ILog
    {
        string Message { get; }
        LogLevel Level { get; }
        DateTime Date { get; }
        Exception Exception { get; }
        string Details { get; }
    }
}
