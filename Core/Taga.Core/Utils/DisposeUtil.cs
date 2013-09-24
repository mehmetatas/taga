using System;

namespace Taga.Core.Utils
{
    public static class DisposeUtil
    {
        public static void DisposeAndCollect<T>(ref T disposable) where T : class, IDisposable
        {
            Dispose(ref disposable);
            GC.Collect(3, GCCollectionMode.Optimized);
        }

        public static void Dispose<T>(ref T disposable) where T : class, IDisposable
        {
            Dispose(disposable);
            disposable = null;
        }

        public static void Dispose(IDisposable disposable)
        {
            disposable.Dispose();
        }
    }
}
