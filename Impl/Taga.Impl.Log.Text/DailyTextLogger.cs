using System;
using Taga.Core.Utils;

namespace Taga.Impl.Log.Text
{
    internal class DailyTextLogger : TextLoggerBase
    {
        protected override string GetFileName()
        {
            return String.Format(TextLogConfig.FileNameFormat, DateTime.Now);
        }

        protected override bool RequiresReinit()
        {
            var tempFileName = GetFileName();
            var currentFileName = IOUtil.GetFileName(FilePath);
            return tempFileName != currentFileName;
        }
    }
}
