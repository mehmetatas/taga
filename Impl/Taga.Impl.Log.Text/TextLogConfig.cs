using System.Text;

namespace Taga.Impl.Log.Text
{
    internal static class TextLogConfig
    {
        internal static string LogDirectory { get; set; }
        internal static string FileNameFormat { get; set; }
        internal static TextLogStrategy Strategy { get; set; }
        internal static int MaxFileSize { get; set; }
        internal static Encoding Encoding { get; set; }
    }
}
