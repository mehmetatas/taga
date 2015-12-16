using System;
using System.Collections;
using System.Linq.Expressions;

namespace Taga.Orm.Dynamix.Impl
{
    public static class PocoFactory
    {
        public static Func<T> CreateFactory<T>()
        {
            return Expression.Lambda<Func<T>>(Expression.New(typeof(T))).Compile();
        }

        public static Func<object> CreateFactory(Type pocoType)
        {
            return (Func<object>)Expression.Lambda(typeof(Func<object>), Expression.New(pocoType)).Compile();
        }

        public static Func<IList> CreateListFactory(Type listType)
        {
            var funType = typeof(Func<>).MakeGenericType(listType);
            return (Func<IList>)Expression.Lambda(funType, Expression.New(listType)).Compile();
        }
    }
}