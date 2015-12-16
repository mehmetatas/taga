using System;
using System.Linq.Expressions;

namespace Taga.Orm.Db
{
    public interface IOrderByQuery<T> : IQueryExecution<T> where T : class, new()
    {
        IOrderByQuery<T> OrderBy(Expression<Func<T, object>> props);

        IOrderByQuery<T> OrderByDesc(Expression<Func<T, object>> props);
    }
}