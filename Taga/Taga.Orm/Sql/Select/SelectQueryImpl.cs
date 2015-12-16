using System.Linq.Expressions;
using Taga.Orm.Dynamix.Impl;
using Taga.Orm.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using Taga.Orm.Sql.Where;
using Taga.Orm.Sql.Where.ExpressionVisitors;

namespace Taga.Orm.Sql.Select
{
    class SelectQueryImpl : ISelectQuery, IWhereExpressionListener
    {
        private readonly TableMeta _fromTableMeta;
        private readonly AliasContext _aliasCtx = new AliasContext();
        private readonly Dictionary<string, Table> _tables = new Dictionary<string, Table>();
        private readonly Dictionary<string, IEnumerable<ColumnMeta>> _outputMappings = new Dictionary<string, IEnumerable<ColumnMeta>>();

        public Table From { get; }
        public IDictionary<string, Column> SelectColumns { get; }
        public IDictionary<string, Join> Joins { get; }
        public List<IWhereExpression> WhereExpressions { get; }
        public List<OrderBy> OrderByColumns { get; }

        public int Page { get; private set; }
        public int PageSize { get; private set; }

        public SelectQueryImpl(TableMeta fromTableMeta)
        {
            _fromTableMeta = fromTableMeta;
            SelectColumns = new Dictionary<string, Column>();
            WhereExpressions = new List<IWhereExpression>();
            Joins = new Dictionary<string, Join>();
            OrderByColumns = new List<OrderBy>();

            From = GetOrAddTable(fromTableMeta.TableName, fromTableMeta);
        }

        Column IWhereExpressionListener.RegisterColumn(IList<ColumnMeta> propChain)
        {
            var colMeta = propChain.Last();

            Table table;
            if (colMeta.Identity && propChain.Count > 1)
            {
                colMeta = propChain[propChain.Count - 2];
                table = EnsureJoined(propChain.Take(propChain.Count - 2));
            }
            else
            {
                table = EnsureJoined(propChain.Take(propChain.Count - 1));
            }

            return new Column
            {
                Meta = colMeta,
                Table = table
            };
        }

        public EntityDeserializer CreateDeserializer()
        {
            return new EntityDeserializer(From.Meta.Factory, _outputMappings);
        }

        public void AddColumn(IList<ColumnMeta> propChain)
        {
            var colMeta = propChain.Last();
            var table = EnsureJoined(propChain.Take(propChain.Count - 1));
            var column = AddColumn(table, colMeta);

            if (!_outputMappings.ContainsKey(column.Alias))
            {
                _outputMappings.Add(column.Alias, propChain);
            }
        }

        public void Join(IList<ColumnMeta> props)
        {
            EnsureJoined(props);
        }

        public void Where<T>(Expression<Func<T, bool>> filter)
        {
            var whereExp = WhereExpressionVisitor.Build(_fromTableMeta.DbMeta, filter, this);
            Where(whereExp);
        }

        public void Where(IWhereExpression whereExp)
        {
            WhereExpressions.Add(whereExp);
        }

        public void OrderBy(IList<ColumnMeta> propChain, bool desc)
        {
            var colMeta = propChain.Last();
            var table = EnsureJoined(propChain.Take(propChain.Count - 1));
            var column = AddColumn(table, colMeta);

            if (OrderByColumns.All(c => c.Column.Alias != column.Alias))
            {
                OrderByColumns.Add(new OrderBy
                {
                    Desc = desc,
                    Column = column
                });
            }
        }

        public void SetPage(int page, int pageSize)
        {
            Page = page;
            PageSize = pageSize;
        }

        public void Top(int top)
        {
            Page = -1;
            PageSize = top;
        }

        private Column AddColumn(Table table, ColumnMeta colMeta)
        {
            var colAlias = $"{table.Alias}_{colMeta.ColumnName}";

            if (SelectColumns.ContainsKey(colAlias))
            {
                return SelectColumns[colAlias];
            }

            var column = new Column
            {
                Alias = colAlias,
                Meta = colMeta,
                Table = table
            };

            SelectColumns.Add(column.Alias, column);

            return column;
        }

        private static string GetTableKey(IEnumerable<ColumnMeta> propChain)
        {
            var u = "";
            var key = "";

            foreach (var leftCol in propChain)
            {
                if (!leftCol.IsRefrence)
                {
                    break;
                }

                var rightCol = leftCol.ReferencedTable.IdColumn;

                key += u + $"{leftCol.Table.TableName}{leftCol.ColumnName}_{rightCol.Table.TableName}{rightCol.ColumnName}";

                u = "_";
            }

            return key;
        }

        private Table GetOrAddTable(string tableKey, TableMeta tableMeta)
        {
            if (_tables.ContainsKey(tableKey))
            {
                return _tables[tableKey];
            }

            var alias = _aliasCtx.GetAlias(tableMeta.TableName);

            var table = new Table
            {
                Alias = alias,
                Meta = tableMeta
            };

            _tables.Add(tableKey, table);

            return table;
        }

        /// <summary>
        /// Ensures a valid join for the property chain exists in the query and 
        /// returns the Table object created for the property.
        /// </summary>
        /// <param name="props">The props.</param>
        /// <returns></returns>
        private Table EnsureJoined(IEnumerable<ColumnMeta> props)
        {
            var leftTable = From;

            var i = 0;
            var leftColMetas = props as ColumnMeta[] ?? props.ToArray();
            foreach (var leftColMeta in leftColMetas)
            {
                if (!leftColMeta.IsRefrence)
                {
                    break;
                }

                var subChain = leftColMetas.Take(++i).ToList();

                var rightTableMeta = leftColMeta.ReferencedTable;
                var rightTableKey = GetTableKey(subChain);
                var rightTable = GetOrAddTable(rightTableKey, rightTableMeta);

                var joinKey = $"{leftTable.Alias}_{rightTable.Alias}";

                if (!Joins.ContainsKey(joinKey))
                {
                    //var leftColumn = AddColumn(leftTable, leftColMeta);
                    var leftColumn = new Column
                    {
                        Table = leftTable,
                        Meta = leftColMeta
                    };

                    var rightColumn = AddColumn(rightTable, rightTableMeta.IdColumn);
                    if (!_outputMappings.ContainsKey(rightColumn.Alias))
                    {
                        _outputMappings.Add(rightColumn.Alias, subChain);
                    }

                    Joins.Add(joinKey, new Join
                    {
                        LeftColumn = leftColumn,
                        RightColumn = rightColumn,
                        Type = JoinType.Inner
                    });
                }

                leftTable = rightTable;
            }

            return leftTable;
        }

        class AliasContext
        {
            private readonly List<string> _aliases = new List<string>();

            public string GetAlias(string key)
            {
                var root = Char.ToString(Char.ToLowerInvariant(key[0]));

                var i = 0;
                var alias = root;
                while (_aliases.Contains(alias))
                {
                    alias = root + (++i);
                }

                _aliases.Add(alias);

                return alias;
            }
        }
    }
}
