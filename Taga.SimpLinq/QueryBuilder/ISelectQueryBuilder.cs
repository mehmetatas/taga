using System;
using System.Linq.Expressions;

namespace Taga.SimpLinq.QueryBuilder
{
    public interface ISelectQueryBuilder
    {
        ISelectQueryBuilder Include<T>(params Expression<Func<T, object>>[] propExpressions)
            where T : class, new();

        ISelectQueryBuilder Exclude<T>(params Expression<Func<T, object>>[] propExpressions)
            where T : class, new();

        ISelectQueryBuilder LeftJoin<TLeft, TRight>(Expression<Func<TLeft, object>> propLeft, Expression<Func<TRight, object>> propRight)
            where TLeft : class, new()
            where TRight : class, new();

        ISelectQueryBuilder InnerJoin<TLeft, TRight>(Expression<Func<TLeft, object>> propLeft, Expression<Func<TRight, object>> propRight)
            where TLeft : class, new()
            where TRight : class, new();

        ISelectQueryBuilder Where<T>(Expression<Func<T, bool>> filter)
            where T : class, new();

        ISelectQueryBuilder OrderBy<T>(Expression<Func<T, object>> propExpression, bool desc = false)
            where T : class, new();

        ISelectQueryBuilder Page(int pageIndex, int pageSize);

        ISelectQuery Build();
    }
}