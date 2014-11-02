using System;
using System.Globalization;
using System.Text;

namespace Taga.Core.Repository.Mapping.NamingConvention
{
    public class MySqlNamingConvention : IDatabaseNamingConvention
    {
        public static readonly CultureInfo EnGb = new CultureInfo("en-GB");

        private readonly bool _plurizeTableName;

        public MySqlNamingConvention(bool plurizeTableName)
        {
            _plurizeTableName = plurizeTableName;
        }

        public string GetTableName(string className)
        {
            var tableName = GetColumnName(className);

            if (!_plurizeTableName)
            {
                return tableName;
            }

            if (tableName.EndsWith("y"))
            {
                tableName = tableName.Substring(0, tableName.Length - 1) + "ies";
            }
            else
            {
                tableName += "s";
            }

            return tableName;
        }

        public string GetColumnName(string propertyName)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < propertyName.Length; i++)
            {
                var c = propertyName[i];
                if (Char.IsLower(c))
                {
                    sb.Append(c);
                }
                else
                {
                    if (i != 0)
                    {
                        sb.Append("_");
                    }
                    sb.Append(Char.ToLower(c, EnGb));
                }
            }

            return sb.ToString();
        }
    }
}
