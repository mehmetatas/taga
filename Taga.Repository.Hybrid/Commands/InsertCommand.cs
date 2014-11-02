using System;
using System.Collections;
using System.Data;
using System.Linq;
using System.Text;
using Taga.Core.Repository;
using Taga.Core.Repository.Mapping;

namespace Taga.Repository.Hybrid.Commands
{
    class InsertCommand : BaseCommand
    {
#if DEBUG
        private readonly Hashtable QueryCache = new Hashtable();
#else
        private static readonly Hashtable QueryCache = new Hashtable();
#endif

        public InsertCommand(object entity)
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

            foreach (var columnMapping in tableMapping.InsertColumns)
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

            var id = HybridDbProvider.Insert(tableMapping.Type, cmd, tableMapping.HasSingleAutoIncrementId);

            if (tableMapping.HasSingleAutoIncrementId)
            {
                var prop = tableMapping.IdColumns[0].PropertyInfo;
                prop.SetValue(Entity, Convert.ChangeType(id, prop.PropertyType));
            }
        }

        private string BuildSqlQuery()
        {
            var databaseMapping = MappingProvider.GetDatabaseMapping();
            var tableMapping = MappingProvider.GetTableMapping(Entity.GetType());

            var sb = new StringBuilder("INSERT INTO ")
                .Append(tableMapping.TableName)
                .Append(" (");

            if (databaseMapping.DbSystem == DbSystem.Oracle && tableMapping.HasSingleAutoIncrementId)
            {
                var columnsToInsert = tableMapping.Columns
                    .Select(col => col.ColumnName)
                    .ToArray();

                var paramNames = tableMapping.InsertColumns
                    .Select(col => col.ColumnName)
                    .Select(GetParamName)
                    .ToArray();

                sb.Append(String.Join(",", columnsToInsert))
                    .Append(") VALUES (")
                    .AppendFormat("SEQ_{0}.nextval,", tableMapping.TableName)
                    .Append(String.Join(",", paramNames))
                    .Append(")");
            }
            else
            {
                var columnsToInsert = tableMapping.InsertColumns
                    .Select(col => col.ColumnName)
                    .ToArray();

                var paramNames = columnsToInsert
                    .Select(GetParamName)
                    .ToArray();

                sb.Append(String.Join(",", columnsToInsert))
                    .Append(") VALUES (")
                    .Append(String.Join(",", paramNames))
                    .Append(")");
            }

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