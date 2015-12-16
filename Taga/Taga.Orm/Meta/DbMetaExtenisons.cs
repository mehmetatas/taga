using System;
using System.Linq.Expressions;

namespace Taga.Orm.Meta
{
    static class DbMetaExtenisons
    {
        public static TableMeta RegisterEntity<T>(this IDbMeta dbMeta) where T : class, new()
        {
            return dbMeta.RegisterEntity(typeof(T));
        }

        public static IDbMeta RegisterModel<T>(this DbMeta dbMeta) where T : class, new()
        {
            return dbMeta.RegisterModel(typeof(T));
        }

        internal static TableMeta GetTable<T>(this IDbMeta dbMeta) where T : class, new()
        {
            return dbMeta.GetTable(typeof(T));
        }

        internal static ColumnMeta GetColumn<T, TProp>(this IDbMeta dbMeta, Expression<Func<T, TProp>> propExp) where T : class, new()
        {
            return dbMeta.GetColumn(propExp.GetPropertyInfo());
        }

        internal static IAssociationMeta GetAssociation<T, TProp>(this IDbMeta dbMeta, Expression<Func<T, TProp>> propExp) where T : class, new()
        {
            return dbMeta.GetAssociation(propExp.GetPropertyInfo());
        }
    }
}