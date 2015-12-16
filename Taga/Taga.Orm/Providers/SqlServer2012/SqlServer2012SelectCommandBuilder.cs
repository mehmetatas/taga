using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Taga.Orm.Meta;
using Taga.Orm.Sql;
using Taga.Orm.Sql.Command;
using Taga.Orm.Sql.Select;
using Taga.Orm.Sql.Where.Expressions;

namespace Taga.Orm.Providers.SqlServer2012
{
    public class SqlServer2012SelectCommandBuilder : ISelectCommandBuilder
    {
        private readonly IDbMeta _meta;
        private readonly StringBuilder _cmd = new StringBuilder();
        private readonly Dictionary<string, CommandParameter> _param = new Dictionary<string, CommandParameter>();

        public SqlServer2012SelectCommandBuilder(IDbMeta meta)
        {
            _meta = meta;
        }

        public Command Build(ISelectQuery query)
        {
            if (query.IsPaging())
            {
                AppendPagingStart();
            }

            AppendSelect();

            if (query.IsTop())
            {
                AppendTop(query);
            }

            AppendColumnsAndFrom(query);

            AppendJoins(query);

            if (query.WhereExpressions.Any())
            {
                AppendWhere(query);
            }

            if (query.IsPaging())
            {
                AppendPagingEnd(query);
            }
            else if (query.OrderByColumns.Any())
            {
                AppendOrderBy(query);
            }

            return Command.TextCommand(_cmd.ToString(), _param);
        }

        private void AppendPagingStart()
        {
            _cmd.AppendLine("WITH __DATA AS (");
        }

        private void AppendSelect()
        {
            _cmd.Append("SELECT");
        }

        private void AppendTop(ISelectQuery query)
        {
            _cmd.AppendFormat(" TOP {0}", query.PageSize);
        }

        private void AppendColumnsAndFrom(ISelectQuery query)
        {
            _cmd.AppendLine()
                .AppendLine(String.Join(",\n", query.SelectColumns.Values.Select(
                    c => $"  {c.Table.Alias}.{c.Meta.ColumnName} {c.Alias}")))
                .AppendFormat("FROM [{0}] {1}", query.From.Meta.TableName, query.From.Alias)
                .AppendLine();
        }

        private void AppendJoins(ISelectQuery query)
        {
            foreach (var join in query.Joins.Select(j => j.Value))
            {
                _cmd.AppendFormat("  {0} JOIN [{1}] {2} ON {3}.{4} = {2}.{5}",
                    join.Type.ToString().ToUpperInvariant(),
                    join.RightColumn.Table.Meta.TableName,
                    join.RightColumn.Table.Alias,
                    join.LeftColumn.Table.Alias,
                    join.LeftColumn.Meta.ColumnName,
                    join.RightColumn.Meta.ColumnName)
                    .AppendLine();
            }
        }

        private void AppendWhere(ISelectQuery query)
        {
            var where = new LogicalExpression
            {
                Operand1 = query.WhereExpressions[0],
                Operator = Operator.And
            };

            foreach (var whereExpression in query.WhereExpressions.Skip(1))
            {
                where.Operand2 = whereExpression;
                where = new LogicalExpression
                {
                    Operand1 = where,
                    Operator = Operator.And
                };
            }

            var whereExp = where.Operand1;
            var builder = _meta.DbProvider.CreateWhereCommandBuilder(_meta);
            whereExp.Accept(builder);
            var whereCmd = builder.Build();

            _cmd.AppendLine("WHERE")
                .AppendLine(whereCmd.CommandText);
            foreach (var sqlParameter in whereCmd.Parameters)
            {
                _param.Add(sqlParameter.Key, sqlParameter.Value);
            }
        }

        private void AppendPagingEnd(ISelectQuery query)
        {
            /*
                 with
                    __DATA as (SELECT...), 
                    __COUNT as (select count(0) as _ROWCOUNT from __DATA)
                select * from __COUNT, __DATA
                order by s.SalesOrderID
                offset 0 rows fetch next 10 rows only*/

            _cmd.AppendLine(
                "),\n__COUNT AS (SELECT COUNT(0) AS __ROWCOUNT FROM __DATA)\nSELECT * FROM __COUNT, __DATA")
                .Append("ORDER BY ");

            if (query.OrderByColumns.Any())
            {
                var comma = "";
                foreach (var col in query.OrderByColumns)
                {
                    _cmd.AppendFormat("{0}__DATA.{1} {2}", comma, col.Column.Alias, col.Desc ? "DESC" : "ASC");
                    comma = ",";
                }
            }
            else
            {
                var fromCol = (query.SelectColumns.Values.FirstOrDefault(c => c.Meta.Identity && c.Table == query.From) ??
                               query.SelectColumns.Values.FirstOrDefault(c => c.Meta.Identity)) ??
                               query.SelectColumns.Values.First();

                _cmd.AppendFormat("__DATA.{0}", fromCol.Alias);
            }

            var offsetParam = new CommandParameter
            {
                Name = "pOffset",
                Value = (query.Page - 1) * query.PageSize,
                ParameterMeta = new ParameterMeta { DbType = DbType.Int32 }
            };

            var limitParam = new CommandParameter
            {
                Name = "pLimit",
                Value = query.PageSize,
                ParameterMeta = new ParameterMeta { DbType = DbType.Int32 }
            };
            
            _param.Add(offsetParam.Name, offsetParam);
            _param.Add(limitParam.Name, limitParam);

            _cmd.AppendLine()
                .Append("OFFSET @pOffset ROWS FETCH NEXT @pLimit ROWS ONLY");
        }

        private void AppendOrderBy(ISelectQuery query)
        {
            _cmd.Append("ORDER BY ");
            var comma = "";
            foreach (var col in query.OrderByColumns)
            {
                _cmd.AppendFormat("{0}{1}.{2} {3}", comma, col.Column.Table.Alias, col.Column.Meta.ColumnName,
                    col.Desc ? "DESC" : "ASC");
                comma = ",";
            }
        }
    }
}