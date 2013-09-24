using System.IO;

namespace Taga.Core.Utils
{
    public static class IOUtil
    {
        public static void CreateFile(string filePath)
        {
            File.Create(filePath).Dispose();
        }

        public static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        public static long GetFileSize(string filePath)
        {
            return new FileInfo(filePath).Length;
        }

        public static string GetFileName(string filePath)
        {
            return Path.GetFileName(filePath);
        }

        public static void FlushAndDispose(ref Stream stream)
        {
            FlushAndClose(stream);
            DisposeUtil.Dispose(ref stream);
        }

        public static void FlushAndClose(Stream stream)
        {
            stream.Flush();
            stream.Close();
        }

        public static void FlushAndClose(TextWriter writer)
        {
            writer.Flush();
            writer.Close();
        }

        public static void FlushAndClose(BinaryWriter writer)
        {
            writer.Flush();
            writer.Close();
        }
    }
}
