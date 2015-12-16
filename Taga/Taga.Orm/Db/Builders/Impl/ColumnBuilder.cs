using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Taga.Orm.Meta;

namespace Taga.Orm.Db.Builders.Impl
{
    class ColumnBuilder<T, TProp> : IColumnBuilder<T, TProp>
        where T : class, new()
    {
        private readonly ColumnMeta _meta;
        private readonly ITableBuilder<T> _parentBuilder;

        public ColumnBuilder(ColumnMeta meta, ITableBuilder<T> parentBuilder)
        {
            _meta = meta;
            _parentBuilder = parentBuilder;
        }

        public IColumnBuilder<T, TProp> ColumnName(string columnName)
        {
            _meta.ColumnName = columnName;
            return this;
        }

        public IColumnBuilder<T, TProp> DbType(DbType dbType)
        {
            _meta.ParameterMeta.DbType = dbType;
            return this;
        }

        public IColumnBuilder<T, TProp> Size(int size)
        {
            _meta.ParameterMeta.Size = size;
            return this;
        }

        public IColumnBuilder<T, TProp> Scale(byte scale)
        {
            _meta.ParameterMeta.Scale = scale;
            return this;
        }

        public IColumnBuilder<T, TProp> Precision(byte precision)
        {
            _meta.ParameterMeta.Precision = precision;
            return this;
        }

        public IColumnBuilder<T, TProp> AutoIncrement()
        {
            _meta.AutoIncrement = true;
            return this;
        }

        public IColumnBuilder<T, TProp> Id()
        {
            _meta.Identity = true;
            return this;
        }
        
        public IColumnBuilder<T, TOtherProp> Column<TOtherProp>(Expression<Func<T, TOtherProp>> propExp) where TOtherProp : class, new()
        {
            return _parentBuilder.Column(propExp);
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