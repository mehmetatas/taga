using System;
using System.Linq;
using System.Reflection;
using Taga.Core.Attributes;
using Taga.Core.Utils;

namespace Taga.Core.Extensions
{
    public static class EnumExtensions
    {
        public static string GetName(this Enum value)
        {
            var type = value.GetType();
            var fieldInfo = type.GetField(value.ToString());
            var attribs = fieldInfo.GetCustomAttributes<NameAttribute>(false).ToArray();

            return ArrayUtil.HasValue(attribs)
                ? attribs[0].Name
                : value.ToString();
        }
    }
}
