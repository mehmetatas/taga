
namespace Taga.Core.Repository.Mapping.NamingConvention
{
    public class SqlServerNamingConvention : IDatabaseNamingConvention
    {
        private readonly bool _plurizeTableName;

        public SqlServerNamingConvention(bool plurizeTableName)
        {
            _plurizeTableName = plurizeTableName;
        }

        public string GetTableName(string className)
        {
            if (!_plurizeTableName)
            {
                return className;
            }

            var tableName = className;

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
            return propertyName;
        }
    }
}
