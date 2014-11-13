using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Taga.Core.Repository;

namespace Taga.Core.ORM
{
    public interface IAzQuery<T>
    {
        IAzQuery<T> Where(Expression<Func<T, object>> propExpression);
        IAzQuery<T> And(Expression<Func<T, object>> propExpression);
        IAzQuery<T> Or(Expression<Func<T, object>> propExpression);
        IAzQuery<T> Not();
        IAzQuery<T> Eq<TParam>(TParam value);
        IAzQuery<T> NotEq<TParam>(TParam value);
        IAzQuery<T> Lt<TParam>(TParam value);
        IAzQuery<T> Lte<TParam>(TParam value);
        IAzQuery<T> Gt<TParam>(TParam value);
        IAzQuery<T> Gte<TParam>(TParam value);
        IAzQuery<T> In<TParam>(params TParam[] values);
        IAzQuery<T> NotIn<TParam>(params TParam[] values);
        IAzQuery<T> Contains(string value);
        IAzQuery<T> StartsWith(string value);
        IAzQuery<T> EndsWith(string value);
        IAzQuery<T> IsNull();
        IAzQuery<T> IsNotNull();
        IAzQuery<T> BeginAnd(Expression<Func<T, object>> propExpression = null);
        IAzQuery<T> BeginOr(Expression<Func<T, object>> propExpression = null);
        IAzQuery<T> BeginNot();
        IAzQuery<T> End();

        IPage<T> Page(int pageIndex, int pageSize);
        IList<T> List();
        T FirstOrDefault();
        int Count();
    }

    class Test
    {
        public void F(IAzQuery<User> query)
        {
            query.Where(u => u.Id).In(1, 2, 3, 4)
                .BeginAnd().BeginNot()
                    .BeginAnd(u => u.Email).Not().Contains("@gmail.com").Or(u => u.Email).StartsWith("mustafa@").End()
                    .BeginOr(u => u.Id).Lt(3).And(u => u.Email).EndsWith("wer").End()
                .End().End();

            var arr = new long[] {1,2,3,4};

            IEnumerable<User> queryable = null;
            var q = queryable.Where(
                u =>
                    arr.Contains(u.Id) &&
                    !((!u.Email.Contains("@gmail.com") || u.Email.StartsWith("mustafa@")) ||
                      (u.Id < 3 && u.Email.EndsWith("wer"))));
        }
    }

    class User
    {
        public virtual long Id { get; set; }
        public virtual string Email { get; set; }
    }
}
