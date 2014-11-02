using Taga.Core.Log.Base;

namespace Taga.Impl.Log.Text
{
    public class TextLogger : LoggerDecorator
    {
        public TextLogger()
            : base(TextLoggerFactory.CreateLogger())
        {
        }
    }
}
