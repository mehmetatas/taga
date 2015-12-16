using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Taga.Orm.Meta;

namespace Taga.Orm.Db.Builders.Impl
{
    class TableBuilder<T> : ITableBuilder<T> where T : class ,new()
    {
        private readonly IDbMeta _dbMeta;
        private readonly TableMeta _meta;
        private readonly IDbBuilder _parentBuilder;

        public TableBuilder(IDbMeta dbMeta, TableMeta meta, IDbBuilder parentBuilder)
        {
            _dbMeta = dbMeta;
            _meta = meta;
            _parentBuilder = parentBuilder;
        }

        public ITableBuilder<T> TableName(string tableName)
        {
            _meta.TableName = tableName;
            return this;
        }

        public IColumnBuilder<T, TProp> Column<TProp>(Expression<Func<T, TProp>> propExp)
        {
            var columnMeta = _dbMeta.GetColumn(propExp);
            return new ColumnBuilder<T, TProp>(columnMeta, this);
        }

        public ITableBuilder<TOther> Table<TOther>() where TOther : class, new()
        {
            return _parentBuilder.Table<TOther>();
        }

        public IDbBuilder OneToMany<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> listPropExp, Expression<Func<TMany, TOne>> foreignPropExp)
            where TOne : class, new()
            where TMany : class, new()
        {
            return _parentBuilder.OneToMany(listPropExp, foreignPropExp);
        }

        public IDbBuilder ManyToMany<TParent, TAssoc>(Expression<Func<TParent, IList>> listPropExp)
            where TParent : class, new()
            where TAssoc : class, new()
        {
            return _parentBuilder.ManyToMany<TParent, TAssoc>(listPropExp);
        }

        public IDbFactory BuildFactory()
        {
            return _parentBuilder.BuildFactory();
        }
    }
}