using System;

namespace Taga.Core.Utils
{
    public static class ArrayUtil
    {
        public static bool HasValue(Array arr)
        {
            return arr != null && arr.Length > 0;
        }
    }
}
