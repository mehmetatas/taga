using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using Taga.Core.Repository.Mapping;

namespace Taga.Repository.Hybrid.Commands
{
    class UpdateCommand : BaseCommand
    {
#if DEBUG
        private readonly Hashtable QueryCache = new Hashtable();
#else
        private static readonly Hashtable QueryCache = new Hashtable();
#endif

        public UpdateCommand(object entity)
            : base(entity)
        {
        }

        public override void Execute(IDbCommand cmd)
        {
            if (QueryCache.ContainsKey(EntityType))
            {
                cmd.CommandText = (string)QueryCache[EntityType];
            }
            else
            {
                cmd.CommandText = BuildSqlQuery();
            }

            var tableMapping = MappingProvider.GetTableMapping(EntityType);

            AddParameters(cmd,  tableMapping.UpdateColumns);
            AddParameters(cmd, tableMapping.IdColumns);

            cmd.ExecuteNonQuery();
        }

        private void AddParameters(IDbCommand cmd, params ColumnMapping[] columnMappings)
        {
            foreach (var columnMapping in columnMappings)
            {
                var param = cmd.CreateParameter();
                var value = columnMapping.PropertyInfo.GetValue(Entity);

                if (columnMapping.PropertyInfo.PropertyType.IsEnum)
                {
                    value = Convert.ToInt32(value);
                }

                param.Value = value;
                param.ParameterName = GetParamName(columnMapping.ColumnName);

                cmd.Parameters.Add(param);
            }
        }

        private string BuildSqlQuery()
        {
            var tableMapping = MappingProvider.GetTableMapping(Entity.GetType());

            var setClause = tableMapping.UpdateColumns
                .Select(col => String.Format("{0}={1}", col.ColumnName, GetParamName(col.ColumnName)))
                .ToArray();

            var whereClause = tableMapping.IdColumns
                .Select(col => String.Format("{0}={1}", col.ColumnName, GetParamName(col.ColumnName)))
                .ToArray();

            var sb = new StringBuilder("UPDATE ")
                .Append(tableMapping.TableName)
                .Append(" SET ")
                .Append(String.Join(",", setClause))
                .Append(" WHERE ")
                .Append(String.Join(" AND ", whereClause));

            var sql = sb.ToString();

            lock (QueryCache)
            {
                if (QueryCache.ContainsKey(EntityType))
                {
                    return (string)QueryCache[EntityType];
                }
                QueryCache.Add(EntityType, sql);
            }

            return sql;
        }
    }
}