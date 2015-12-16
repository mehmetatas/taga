using System;
using System.Linq.Expressions;

namespace Taga.Orm.Db
{
    public interface IWhereQuery<T> : IOrderByQuery<T> where T : class, new()
    {
        IWhereQuery<T> Where(Expression<Func<T, bool>> filter);
    }


    public static class WhereQueryExtensions
    {
        public static T FirstOrDefault<T>(this IWhereQuery<T> query, Expression<Func<T, bool>> filter) where T : class, new()
        {
            return query.Where(filter).FirstOrDefault();
        }
    }
}