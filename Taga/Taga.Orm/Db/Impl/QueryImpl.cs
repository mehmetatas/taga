using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using Taga.Orm.Meta;
using Taga.Orm.Sql;
using Taga.Orm.Sql.Command;
using Taga.Orm.Sql.Select;
using Taga.Orm.Sql.Where;

namespace Taga.Orm.Db.Impl
{
    class QueryImpl<T> : IQuery<T>, ISelectQueryBuilder where T : class, new()
    {
        private readonly IDbMeta _meta;
        private readonly ICommandExecutor _queryExecuter;
        private readonly SelectQueryImpl _query;
        private bool _autoIncludeFromColumns;

        internal string FromTableAlias => _query.From.Alias;

        public QueryImpl(IDbMeta meta, ICommandExecutor queryExecuter)
        {
            _query = new SelectQueryImpl(meta.GetTable<T>());
            _meta = meta;
            _queryExecuter = queryExecuter;
            _autoIncludeFromColumns = true;
        }

        public IQuery<T> Join<TProp>(Expression<Func<T, TProp>> refProp, Expression<Func<TProp, object>> include = null) where TProp : class ,new()
        {
            var propChain = refProp.GetPropertyChain(_meta);

            // Build join
            _query.Join(propChain);

            if (include == null)
            {
                // No columns specified Include all properties of joined type
                Include(true, refProp.GetMemberExpression());
            }
            else
            {
                if (include.Body is NewExpression)
                {
                    // Include multiple properties
                    Include(true, include.Body as NewExpression, propChain);
                }
                else
                {
                    // Include specified property only
                    Include(true, include.GetMemberExpression(), propChain);
                }
            }

            return this;
        }

        internal void Join<TProp>(List<ColumnMeta> propChain, Expression<Func<TProp, object>> include = null) where TProp : class ,new()
        {
            _query.Join(propChain);

            if (include == null)
            {
                // No columns specified Include all properties of joined type
                Include(true, propChain);
            }
            else
            {
                if (include.Body is NewExpression)
                {
                    // Include multiple properties
                    Include(true, include.Body as NewExpression, propChain);
                }
                else
                {
                    // Include specified property only
                    Include(true, include.GetMemberExpression(), propChain);
                }
            }
        }

        public IQuery<T> Include(Expression<Func<T, object>> propExpression)
        {
            if (propExpression.Body is NewExpression)
            {
                Include(false, propExpression.Body as NewExpression);
            }
            else
            {
                Include(false, propExpression.GetMemberExpression());
            }
            return this;
        }

        private void Include(bool join, NewExpression newExp, List<ColumnMeta> rootChain = null)
        {
            var includedProps = newExp.Arguments.Cast<MemberExpression>();
            foreach (var includeProp in includedProps)
            {
                Include(join, includeProp, rootChain);
            }
        }

        private void Include(bool join, MemberExpression memberExp, IEnumerable<ColumnMeta> rootChain = null)
        {
            var list = rootChain == null
                ? new List<ColumnMeta>()
                : new List<ColumnMeta>(rootChain);

            list.AddRange(memberExp.GetPropertyChain(_meta, false));
            
            Include(join, list);
        }

        private void Include(bool join, List<ColumnMeta> propChain)
        {
            _query.AddColumn(propChain);

            var colMeta = propChain.Last();

            if (colMeta.IsRefrence)
            {
                foreach (var columnMeta in colMeta.ReferencedTable.Columns)
                {
                    _query.AddColumn(new List<ColumnMeta>(propChain) { columnMeta });
                }
            }

            if (!join && colMeta.Table == _query.From.Meta)
            {
                _autoIncludeFromColumns = false;
            }
        }

        public IWhereQuery<T> Where(Expression<Func<T, bool>> filter)
        {
            _query.Where(filter);
            return this;
        }

        internal void Where(IWhereExpression whereExp)
        {
            _query.Where(whereExp);
        }

        public IOrderByQuery<T> OrderBy(Expression<Func<T, object>> props)
        {
            _query.OrderBy(props.GetPropertyChain(_meta, false), false);
            return this;
        }

        public IOrderByQuery<T> OrderByDesc(Expression<Func<T, object>> props)
        {
            _query.OrderBy(props.GetPropertyChain(_meta,false), true);
            return this;
        }

        public T FirstOrDefault()
        {
            _query.Top(1);

            using (var reader = ExecuteReader())
            {
                if (!reader.Read())
                {
                    return null;
                }

                var mapper = _query.CreateDeserializer();
                return mapper.Deserialize(reader) as T;
            }
        }

        public List<T> ToList()
        {
            using (var reader = ExecuteReader())
            {
                var list = new List<T>();
                var mapper = _query.CreateDeserializer();

                while (reader.Read())
                {
                    list.Add((T)mapper.Deserialize(reader));
                }

                return list;
            }
        }

        public Page<T> Page(int page, int pageSize)
        {
            _query.SetPage(page, pageSize);

            using (var reader = ExecuteReader())
            {
                var list = new List<T>();
                var mapper = _query.CreateDeserializer();

                var totalCount = 0;

                while (reader.Read())
                {
                    if (totalCount == 0)
                    {
                        totalCount = Convert.ToInt32(reader["__ROWCOUNT"]);
                    }
                    list.Add((T)mapper.Deserialize(reader));
                }

                return new Page<T>(page, pageSize, totalCount, list);
            }
        }

        public Page<T> Top(int top)
        {
            _query.Top(top + 1);

            var list = ToList();

            return new Page<T>(1, top, list.Count, list.Take(top));
        }

        public ISelectQuery Build()
        {
            // If there is no column specified from the From table select all columns by default.
            if (_autoIncludeFromColumns) // && _query.SelectColumns.All(c => c.Value.Table != _query.From)
            {
                var fromTable = _query.From.Meta;
                foreach (var columnMeta in fromTable.Columns)
                {
                    _query.AddColumn(new[] { columnMeta });
                }
            }

            // If Id columns of From table are not included, include all id columns by default
            foreach (var idColumn in _query.From.Meta.Columns.Where(c => c.Identity))
            {
                if (_query.SelectColumns.All(c => c.Value.Meta != idColumn))
                {
                    _query.AddColumn(new[] { idColumn });
                }
            }

            return _query;
        }

        private IDataReader ExecuteReader()
        {
            var query = BuildCommand();
            return _queryExecuter.ExecuteReader(query);
        }

        private Command BuildCommand()
        {
            var query = Build();
            return _meta.DbProvider.CreateSelectCommandBuilder(_meta).Build(query);
        }
    }
}