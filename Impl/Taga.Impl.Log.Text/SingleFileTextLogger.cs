
namespace Taga.Impl.Log.Text
{
    internal class SingleFileTextLogger : TextLoggerBase
    {
        protected override string GetFileName()
        {
            return TextLogConfig.FileNameFormat;
        }

        protected override bool RequiresReinit()
        {
            return false;
        }
    }
}
