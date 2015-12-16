using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Taga.Orm.Db.Builders
{
    public interface IDbBuilder
    {
        ITableBuilder<T> Table<T>() where T : class, new();

        IDbBuilder OneToMany<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> listPropExp, Expression<Func<TMany, TOne>> foreignPropExp)
            where TOne : class, new()
            where TMany : class, new();

        IDbBuilder ManyToMany<TParent, TAssoc>(Expression<Func<TParent, IList>> listPropExp)
            where TParent : class, new()
            where TAssoc : class, new();

        IDbFactory BuildFactory();
    }
}
