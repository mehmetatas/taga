using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace Taga.Orm.Db.Builders
{
    public interface IColumnBuilder<T, TProp>
        where T : class, new()
    {
        IColumnBuilder<T, TProp> ColumnName(string columnName);
        IColumnBuilder<T, TProp> DbType(DbType dbType);
        IColumnBuilder<T, TProp> Size(int size);
        IColumnBuilder<T, TProp> Scale(byte scale);
        IColumnBuilder<T, TProp> Precision(byte precision);
        IColumnBuilder<T, TProp> AutoIncrement();
        IColumnBuilder<T, TProp> Id();

        IColumnBuilder<T, TOtherProp> Column<TOtherProp>(Expression<Func<T, TOtherProp>> propExp) where TOtherProp : class, new();
        ITableBuilder<TOther> Table<TOther>() where TOther : class, new();
        IDbBuilder OneToMany<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> listPropExp, Expression<Func<TMany, TOne>> foreignPropExp)
            where TOne : class, new()
            where TMany : class, new();
        IDbBuilder ManyToMany<TParent, TAssoc>(Expression<Func<TParent, IList>> listPropExp)
            where TParent : class, new()
            where TAssoc : class, new();
        IDbFactory BuildFactory();
    }
}