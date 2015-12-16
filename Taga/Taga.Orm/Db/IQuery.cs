using System;
using System.Linq.Expressions;

namespace Taga.Orm.Db
{
    public interface IQuery<T> : IWhereQuery<T> where T : class, new()
    {
        IQuery<T> Join<TProp>(Expression<Func<T, TProp>> refProp,
            Expression<Func<TProp, object>> include = null) where TProp : class, new();

        IQuery<T> Include(Expression<Func<T, object>> propExpression);
    }
}