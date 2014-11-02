using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using Taga.Core.Repository.Mapping;

namespace Taga.Repository.Hybrid.Commands
{
    class DeleteCommand : BaseCommand
    {
#if DEBUG
        private readonly Hashtable QueryCache = new Hashtable();
#else
        private static readonly Hashtable QueryCache = new Hashtable();
#endif

        public DeleteCommand(object entity)
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

            foreach (var columnMapping in tableMapping.IdColumns)
            {
                var param = cmd.CreateParameter();

                param.Value = columnMapping.PropertyInfo.GetValue(Entity);
                param.ParameterName = GetParamName(columnMapping.ColumnName);

                cmd.Parameters.Add(param);
            }

            cmd.ExecuteNonQuery();
        }

        private string BuildSqlQuery()
        {
            var tableMapping = MappingProvider.GetTableMapping(Entity.GetType());

            var whereClause = tableMapping.IdColumns
                .Select(col => String.Format("{0}={1}", col.ColumnName, GetParamName(col.ColumnName)))
                .ToArray();

            var sb = new StringBuilder("DELETE FROM ")
                .Append(tableMapping.TableName)
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