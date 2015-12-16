using System;
using System.Collections;
using System.Globalization;

namespace Taga.Framework.Utils
{
    public static class QueryStringUtil
    {
        private static readonly CultureInfo EnGb = Cultures.EnGb;

        private const string DateTimeFormat = "O";

        private const int DecimalPrecision = 6;

        private static readonly string DecimalFormat = "##.".PadRight(3 + DecimalPrecision, '0');

        private static readonly IDictionary DefaultValues = new Hashtable();

        public static string ToString(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            var type = RemoveNullable(obj.GetType());

            if (type == typeof(decimal))
            {
                return ToString((decimal)obj);
            }

            if (type == typeof(DateTime))
            {
                return ToString((DateTime)obj);
            }

            return obj.ToString();
        }

        public static object Parse(string str, Type type)
        {
            if (str == null)
            {
                return GetDefaultValue(type);
            }

            var nonNullableType = RemoveNullable(type);

            if (nonNullableType == typeof(decimal))
            {
                return ParseDecimal(str);
            }

            if (nonNullableType == typeof(DateTime))
            {
                return ParseDateTime(str);
            }

            if (nonNullableType == typeof(Guid))
            {
                return new Guid(str);
            }

            if (nonNullableType.IsEnum)
            {
                if (String.IsNullOrEmpty(str))
                {
                    if (IsNullable(type))
                    {
                        return null;
                    }
                    return GetDefaultValue(nonNullableType);
                }
                return Enum.Parse(nonNullableType, str);
            }

            return Convert.ChangeType(str, nonNullableType);
        }

        private static string ToString(DateTime dateTime)
        {
            return dateTime.ToString(DateTimeFormat, EnGb);
        }

        private static string ToString(decimal dec)
        {
            return dec.ToString(DecimalFormat, EnGb);
        }

        private static DateTime ParseDateTime(string str)
        {
            return DateTime.ParseExact(str, DateTimeFormat, EnGb);
        }

        private static decimal ParseDecimal(string str)
        {
            return Math.Round(Decimal.Parse(str, EnGb), DecimalPrecision);
        }

        private static Type RemoveNullable(Type type)
        {
            if (IsNullable(type))
            {
                type = type.GetGenericArguments()[0];
            }
            return type;
        }

        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private static object GetDefaultValue(Type type)
        {
            if (!type.IsValueType)
            {
                return null;
            }

            lock (DefaultValues)
            {
                if (DefaultValues.Contains(type))
                {
                    return DefaultValues[type];
                }

                var defaultValue = Activator.CreateInstance(type);
                DefaultValues.Add(type, defaultValue);
                return defaultValue;
            }
        }
    }
}
