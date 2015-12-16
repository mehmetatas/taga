using System;
using System.Collections;
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
    class OneToManyLoader : IAssociationLoader
    {
        private readonly OneToManyMeta _meta;

        public OneToManyLoader(OneToManyMeta meta)
        {
            _meta = meta;
        }

        public void Load<T, TProp>(IList<T> parentEntities, ICommandExecutor cmdExec, Expression<Func<TProp, object>> includeProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            var query = new QueryImpl<TProp>(_meta.DbMeta, cmdExec);

            var parentId = _meta.PrimaryKey;
            var parentIdGetter = parentId.GetterSetter;

            var parentIds = parentEntities.Select(parentIdGetter.Get).Distinct();

            query.Where(new InExpression
            {
                Column = new ColumnExpression
                {
                    Column = new Column
                    {
                        Table = new Table
                        {
                            Meta = _meta.ForeignKey.Table, 
                            Alias = query.FromTableAlias
                        },
                        Meta = _meta.ForeignKey
                    }
                },
                Values = new ValueExpression
                {
                    ColumnMeta = _meta.ForeignKey,
                    Value = parentIds
                }
            });

            query.Include(includeProps);

            var children = query.ToList();

            foreach (var child in children)
            {
                var parentObj = _meta.ForeignKey.GetterSetter.Get(child);
                var parentIdValue = _meta.ForeignKey.ReferencedTable.IdColumn.GetterSetter.Get(parentObj);

                var parentEntity = parentEntities.First(pe => parentIdGetter.Get(pe).Equals(parentIdValue));

                var list = (IList)_meta.ListGetterSetter.Get(parentEntity);
                if (list == null)
                {
                    list = _meta.ListFactory();
                    _meta.ListGetterSetter.Set(parentEntity, list);
                }

                list.Add(child);
                _meta.ForeignKey.GetterSetter.Set(child, parentEntity);
            }
        }
    }
}