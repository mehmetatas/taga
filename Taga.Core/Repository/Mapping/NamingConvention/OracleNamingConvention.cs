using System;
using System.Globalization;
using System.Text;

namespace Taga.Core.Repository.Mapping.NamingConvention
{
    public class OracleNamingConvention : IDatabaseNamingConvention
    {
        public static readonly CultureInfo EnGb = new CultureInfo("en-GB");

        private readonly bool _plurizeTableName;

        public OracleNamingConvention(bool plurizeTableName)
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

            if (tableName.EndsWith("Y"))
            {
                tableName = tableName.Substring(0, tableName.Length - 1) + "IES";
            }
            else
            {
                tableName += "S";
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
                    sb.Append(Char.ToUpper(c, EnGb));
                }
                else
                {
                    if (i != 0)
                    {
                        sb.Append("_");
                    }
                    sb.Append(c);
                }
            }

            return sb.ToString();
        }
    }
}
