using System;

namespace Taga.Impl.Log.Text
{
    internal class FileSizeTextLogger : TextLoggerBase
    {
        private long _currentFileSize;

        protected override string GetFileName()
        {
            return String.Format(TextLogConfig.FileNameFormat, DateTime.Now);
        }

        protected override bool RequiresReinit()
        {
            var requiresReinit = _currentFileSize >= TextLogConfig.MaxFileSize;
            if (requiresReinit)
                _currentFileSize = 0;
            return requiresReinit;
        }

        protected override void WriteLog(Core.Log.ILog log)
        {
            _currentFileSize += log.GetText().Length;
            base.WriteLog(log);
        }
    }
}
