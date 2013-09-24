using System;
using System.IO;
using Taga.Core.Log;
using Taga.Core.Log.Base;
using Taga.Core.Utils;

namespace Taga.Impl.Log.Text
{
    internal abstract class TextLoggerBase : Logger
    {
        protected readonly object LockObj = new Object();

        protected string FilePath;
        protected volatile StreamWriter Writer;

        protected override void WriteLog(ILog log)
        {
            EnsureWriter();
            lock (LockObj)
                Writer.WriteLine(log.GetText());
        }

        private void EnsureWriter()
        {
            if (Init())
                return;

            if (RequiresReinit())
                Reinit();
        }

        private bool Init()
        {
            if (Writer == null)
            {
                lock (LockObj)
                {
                    if (Writer == null)
                    {
                        InitWriter();
                        return true;
                    }
                }
            }
            return false;
        }

        private void Reinit()
        {
            CloseWriter();
            InitWriter();
        }

        private void CloseWriter()
        {
            lock (LockObj)
                IOUtil.FlushAndClose(Writer);
        }

        private void InitWriter()
        {
            FilePath = StringUtil.CombinePaths(TextLogConfig.LogDirectory, GetFileName());
            Writer = new StreamWriter(FilePath, true, TextLogConfig.Encoding);
        }

        protected abstract bool RequiresReinit();
        protected abstract string GetFileName();
    }
}
