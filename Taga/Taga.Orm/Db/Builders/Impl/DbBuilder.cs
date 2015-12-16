using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Taga.Orm.Db.Impl;
using Taga.Orm.Meta;
using Taga.Orm.Providers;

namespace Taga.Orm.Db.Builders.Impl
{
    class DbBuilder : IDbBuilder
    {
        private readonly IDbMeta _meta;

        public DbBuilder(IDbProvider provider)
        {
            _meta = new DbMeta(provider);
        }

        public ITableBuilder<T> Table<T>() where T : class, new()
        {
            var tableMeta = _meta.RegisterEntity<T>();
            return new TableBuilder<T>(_meta, tableMeta, this);
        }

        public IDbBuilder OneToMany<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> listPropExp, Expression<Func<TMany, TOne>> foreignPropExp)
            where TOne : class, new()
            where TMany : class, new()
        {
            _meta.OneToMany(listPropExp, foreignPropExp);
            return this;
        }

        public IDbBuilder ManyToMany<TParent, TAssoc>(Expression<Func<TParent, IList>> listPropExp)
            where TParent : class, new()
            where TAssoc : class, new()
        {
            _meta.ManyToMany<TParent, TAssoc>(listPropExp);
            return this;
        }

        public IDbFactory BuildFactory()
        {
            return new DbFactoryImpl(_meta);
        }
    }
}