using System;
using System.IO;

namespace Taga.Core.Utils
{
    public static class StringUtil
    {
        public static bool HasValue(string str)
        {
            return !String.IsNullOrWhiteSpace(str);
        }

        public static string CombinePaths(params string[] paths)
        {
            return ArrayUtil.HasValue(paths) 
                ? Path.Combine(paths) 
                : String.Empty;
        }
    }
}
