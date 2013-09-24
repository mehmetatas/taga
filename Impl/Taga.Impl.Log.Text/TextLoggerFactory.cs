using Taga.Core.Log;

namespace Taga.Impl.Log.Text
{
    internal static class TextLoggerFactory
    {
        internal static ILogger CreateLogger()
        {
            return LogConfig.EnableBuffer
                       ? new BufferedTextLogger(CreateStrategyLogger()) as ILogger
                       : CreateStrategyLogger();
        }

        private static TextLoggerBase CreateStrategyLogger()
        {
            switch (TextLogConfig.Strategy)
            {
                case TextLogStrategy.Size:
                    return new FileSizeTextLogger();
                case TextLogStrategy.Daily:
                    return new DailyTextLogger();
                default:
                    return new SingleFileTextLogger();
            }
        }
    }
}
