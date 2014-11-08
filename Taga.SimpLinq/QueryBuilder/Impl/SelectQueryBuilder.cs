using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Taga.SimpLinq.QueryBuilder.Impl
{
    class SelectQueryBuilder : ISelectQueryBuilder
    {
        private readonly SelectQuery _query;
        private readonly IPropertyFilter _filter;

        public SelectQueryBuilder(Type fromType, IPropertyFilter filter)
        {
            _filter = filter;
            _query = new SelectQuery(fromType);
            _query.SelectProperties.AddRange(fromType.GetProperties().Where(propInf => !_filter.Ignore(propInf)));
        }

        public ISelectQueryBuilder Include<T>(params Expression<Func<T, object>>[] propExpressions) where T : class, new()
        {
            var type = typeof(T);

            for (var i = 0; i < _query.SelectProperties.Count; i++)
            {
                if (_query.SelectProperties[i].DeclaringType == type)
                {
                    _query.SelectProperties.RemoveAt(i--);
                }
            }

            var propInfos = propExpressions.Length > 0
                    ? propExpressions.Select(GetPropertyInfo).ToList()
                    : type.GetProperties().ToList();

            _query.SelectProperties.AddRange(propInfos.Where(propInf => !_filter.Ignore(propInf)));

            return this;
        }

        public ISelectQueryBuilder Exclude<T>(params Expression<Func<T, object>>[] propExpressions) where T : class, new()
        {
            var type = typeof(T);

            for (var i = 0; i < _query.SelectProperties.Count; i++)
            {
                if (_query.SelectProperties[i].DeclaringType == type)
                {
                    _query.SelectProperties.RemoveAt(i--);
                }
            }

            if (propExpressions.Length == 0)
            {
                return this;
            }

            var excludedProps = propExpressions.Select(GetPropertyInfo).ToList();

            var propInfos = type.GetProperties()
                .Where(prop => !excludedProps.Contains(prop))
                .ToList();

            _query.SelectProperties.AddRange(propInfos.Where(propInf => !_filter.Ignore(propInf)));

            return this;
        }

        public ISelectQueryBuilder LeftJoin<TLeft, TRight>(Expression<Func<TLeft, object>> propLeft, Expression<Func<TRight, object>> propRight)
            where TLeft : class, new()
            where TRight : class, new()
        {
            return Join(propLeft, propRight, JoinType.Left);
        }

        public ISelectQueryBuilder InnerJoin<TLeft, TRight>(Expression<Func<TLeft, object>> propLeft, Expression<Func<TRight, object>> propRight)
            where TLeft : class, new()
            where TRight : class, new()
        {
            return Join(propLeft, propRight, JoinType.Inner);
        }

        public ISelectQueryBuilder RightJoin<TLeft, TRight>(Expression<Func<TLeft, object>> propLeft, Expression<Func<TRight, object>> propRight)
            where TLeft : class, new()
            where TRight : class, new()
        {
            return Join(propLeft, propRight, JoinType.Right);
        }

        public ISelectQueryBuilder Where<T>(Expression<Func<T, bool>> filter) where T : class, new()
        {
            var where = (Where)WhereExpressionBuilder.Build(filter);

            if (_query.Where != null)
            {
                var newWhere = new Where
                {
                    Operator = Operator.And
                };

                newWhere.SetOperand(_query.Where);
                newWhere.SetOperand(where);

                where = newWhere;
            }

            _query.Where = where;

            return this;
        }

        public ISelectQueryBuilder OrderBy<T>(Expression<Func<T, object>> propExpression, bool desc = false) where T : class, new()
        {
            _query.OrderByProperties.Add(new OrderBy
            {
                OrderProperty = GetPropertyInfo(propExpression),
                Descending = desc
            });
            return this;
        }

        public ISelectQuery FirstOrDefault<T>(Expression<Func<T, bool>> filter = null) where T : class, new()
        {
            if (filter != null)
            {
                Where(filter);
            }

            Page(1, 1);

            return _query;
        }

        public ISelectQuery Page(int pageIndex, int pageSize)
        {
            _query.Page = new Page
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return _query;
        }

        public ISelectQuery List()
        {
            return _query;
        }

        private ISelectQueryBuilder Join<TLeft, TRight>(Expression<Func<TLeft, object>> propLeft, Expression<Func<TRight, object>> propRight, JoinType joinType)
        {
            _query.JoinProperties.Add(new Join
            {
                LeftProperty = GetPropertyInfo(propLeft),
                RightProperty = GetPropertyInfo(propRight),
                JoinType = joinType
            });
            return this;
        }

        private static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> propExpression)
        {
            MemberExpression memberExp;

            if (propExpression.Body is UnaryExpression)
            {
                memberExp = (MemberExpression)((UnaryExpression)propExpression.Body).Operand;
            }
            else
            {
                memberExp = (MemberExpression)propExpression.Body;
            }

            return (PropertyInfo)memberExp.Member;
        }
    }
}