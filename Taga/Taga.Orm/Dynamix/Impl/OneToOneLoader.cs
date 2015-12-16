using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Taga.Orm.Db;
using Taga.Orm.Db.Impl;
using Taga.Orm.Meta;
using Taga.Orm.Sql;
using Taga.Orm.Sql.Where.Expressions;

namespace Taga.Orm.Dynamix.Impl
{
    class OneToOneLoader : IAssociationLoader
    {
        private readonly ColumnMeta _refProp;

        public OneToOneLoader(ColumnMeta refProp)
        {
            _refProp = refProp;
        }

        public void Load<T, TProp>(IList<T> parentEntities, ICommandExecutor cmdExec, Expression<Func<TProp, object>> includeProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            var childObjGetterSetter = _refProp.GetterSetter;

            parentEntities = parentEntities.Where(e => childObjGetterSetter.Get(e) != null).ToList();

            var childIdGetter = _refProp.ReferencedTable.IdColumn.GetterSetter;

            var childIds = parentEntities.Select(e => childIdGetter.Get(childObjGetterSetter.Get(e))).Distinct();

            var children = GetChildren(cmdExec, includeProps, childIds);

            foreach (var child in children)
            {
                var childId = childIdGetter.Get(child);
                foreach (var parentEntity in parentEntities.Where(e => childIdGetter.Get(childObjGetterSetter.Get(e)).Equals(childId)))
                {
                    childObjGetterSetter.Set(parentEntity, child);
                }
            }
        }

        private IList<TProp> GetChildren<TProp>(ICommandExecutor cmdExec, Expression<Func<TProp, object>> includeProps, IEnumerable<object> childIds)
            where TProp : class, new()
        {
            var query = new QueryImpl<TProp>(_refProp.DbMeta, cmdExec);

            query.Where(new InExpression
            {
                Column = new ColumnExpression
                {
                    Column = new Column
                    {
                        Table = new Table
                        {
                            Meta = _refProp.ReferencedTable,
                            Alias = query.FromTableAlias
                        },
                        Meta = _refProp.ReferencedTable.IdColumn
                    }
                },
                Values = new ValueExpression
                {
                    ColumnMeta = _refProp.ReferencedTable.IdColumn,
                    Value = childIds
                }
            });

            query.Include(includeProps);

            return query.ToList();
        }
    }
}