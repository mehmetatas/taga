using System;
using System.Globalization;
using System.Threading;
using Taga.Core.Utils;

namespace Taga.Core.Conversion
{
    public static class ConversionHelper
    {
        public static string ToBase64(byte[] bytes)
        {
            return ArrayUtil.HasValue(bytes)
                ? Convert.ToBase64String(bytes)
                : String.Empty;
        }

        public static string ToString(DateTime dateTime, string format)
        {
            return ToString(dateTime, format, Thread.CurrentThread.CurrentCulture);
        }

        public static string ToString(DateTime dateTime, string format, CultureInfo cultureInfo)
        {
            return dateTime.ToString(format, cultureInfo);
        }
    }
}
