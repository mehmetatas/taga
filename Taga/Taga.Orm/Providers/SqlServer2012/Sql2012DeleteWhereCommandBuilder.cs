using Taga.Orm.Meta;
using Taga.Orm.Sql;
using Taga.Orm.Sql.Command;
using Taga.Orm.Sql.Delete;
using Taga.Orm.Sql.Where;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Taga.Orm.Providers.SqlServer2012
{
    public class Sql2012DeleteWhereCommandBuilder : IDeleteManyCommandBuilder, IWhereExpressionListener
    {
        private readonly IDbMeta _meta;
        private TableMeta _table;

        public Sql2012DeleteWhereCommandBuilder(IDbMeta meta)
        {
            _meta = meta;
        }

        public Command Build<T>(Expression<Func<T, bool>> filter) where T : class, new()
        {
            _table = _meta.GetTable<T>();

            var whereBuilder = _meta.DbProvider.CreateWhereCommandBuilder(_meta);

            var cmd = whereBuilder.Build(_meta, filter, this);

            var cmdBuilder = new CommandBuilder()
                .AppendFormat("DELETE FROM [{0}] WHERE ", _table.TableName)
                .Append(cmd.CommandText);

            foreach (var parameter in cmd.Parameters.Values)
            {
                cmdBuilder.AddParameter(parameter);
            }

            return cmdBuilder.Build();
        }

        public Column RegisterColumn(IList<ColumnMeta> propChain)
        {
            var requiresJoin = (propChain.Count > 2) ||
                               (propChain.Count == 2 && !propChain[1].Identity);

            if (requiresJoin)
            {
                throw new NotSupportedException("Joins are not supported by DeleteMany!");
            }

            var prop = propChain[0];
            
            return new Column
            {
                Meta = prop,
                Table = new Table
                {
                    Meta = _table
                }
            };
        }
    }
}
