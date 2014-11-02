using Taga.Core.Log.Base;

namespace Taga.Impl.Log.Text
{
    internal class BufferedTextLogger : BufferedLogger
    {
        internal BufferedTextLogger(TextLoggerBase logger)
            : base(logger)
        {
        }
    }
}
