using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Taga.Core.Repository.SimpLinq
{
    public interface ISimpLinqQuery<T> where T : class, new()
    {
        ISimpLinqQuery<T> Include(params Expression<Func<T, object>>[] propExpressions);

        ISimpLinqQuery<T> Exclude(params Expression<Func<T, object>>[] propExpressions);

        ISimpLinqQuery<T> Where(Expression<Func<T, bool>> filter);

        ISimpLinqQuery<T> OrderBy(Expression<Func<T, object>> propExpression, bool desc = false);

        T FirstOrDefault(Expression<Func<T, bool>> filter = null);

        IPage<T> Page(int pageIndex, int pageSize);

        IList<T> List();

        ISimpLinqQuery<T> LeftJoin<TJoined>(Expression<Func<T, object>> propLeft, Expression<Func<TJoined, object>> propRight)
            where TJoined : class, new();

        ISimpLinqQuery<T> InnerJoin<TJoined>(Expression<Func<T, object>> propLeft, Expression<Func<TJoined, object>> propRight)
            where TJoined : class, new();

        ISimpLinqQuery<T> RightJoin<TJoined>(Expression<Func<T, object>> propLeft, Expression<Func<TJoined, object>> propRight)
            where TJoined : class, new();

        ISimpLinqQuery<T> Include<TJoined>(params Expression<Func<TJoined, object>>[] propExpressions)
            where TJoined : class, new();

        ISimpLinqQuery<T> Exclude<TJoined>(params Expression<Func<TJoined, object>>[] propExpressions)
            where TJoined : class, new();

        ISimpLinqQuery<T> Where<TJoined>(Expression<Func<TJoined, bool>> filter)
            where TJoined : class, new();

        ISimpLinqQuery<T> OrderBy<TJoined>(Expression<Func<TJoined, object>> propExpression, bool desc = false)
            where TJoined : class, new();

        ISimpLinqSelect<T, T1> SelectWith<T1>()
            where T1 : class, new();

        ISimpLinqSelect<T, T1, T2> SelectWith<T1, T2>()
            where T1 : class, new()
            where T2 : class, new();
    }

    public interface ISimpLinqSelect<T1, T2>
        where T1 : class, new()
        where T2 : class, new()
    {
        Tuple<T1, T2> FirstOrDefault(Expression<Func<T1, bool>> filter1 = null,
            Expression<Func<T2, bool>> filter2 = null);

        IPage<Tuple<T1, T2>> Page(int pageIndex, int pageSize);

        IList<Tuple<T1, T2>> List();
    }

    public interface ISimpLinqSelect<T1, T2, T3>
        where T1 : class, new()
        where T2 : class, new()
        where T3 : class, new()
    {
        Tuple<T1, T2, T3> FirstOrDefault(Expression<Func<T1, bool>> filter1 = null,
            Expression<Func<T2, bool>> filter2 = null, Expression<Func<T3, bool>> filter3 = null);

        IPage<Tuple<T1, T2, T3>> Page(int pageIndex, int pageSize);

        IList<Tuple<T1, T2, T3>> List();
    }
}