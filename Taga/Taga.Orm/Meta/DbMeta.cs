using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Taga.Orm.Dynamix.Impl;
using Taga.Orm.Providers;
using Taga.Orm.Sql.Command;

namespace Taga.Orm.Meta
{
    class DbMeta : IDbMeta
    {
        private readonly Hashtable _tables = new Hashtable();
        private readonly Hashtable _columns = new Hashtable();
        private readonly Hashtable _associations = new Hashtable();

        public IDbProvider DbProvider { get; }

        public DbMeta (IDbProvider provider)
        {
            DbProvider = provider;
        }

        public TableMeta[] GetTables() => _tables.Cast<TableMeta>().ToArray();

        public OneToManyMeta OneToMany<TOne, TMany>(Expression<Func<TOne, IEnumerable<TMany>>> listPropExp, Expression<Func<TMany, TOne>> foreignPropExp)
            where TOne : class, new()
            where TMany : class, new()
        {
            EnsureReferences();

            var parentTable = this.GetTable<TOne>();
            var primaryKey = parentTable.IdColumn;

            var listProp = listPropExp.GetPropertyInfo();

            var childCol = GetColumn(foreignPropExp.GetPropertyInfo());

            var assoc = new OneToManyMeta(this)
            {
                ListFactory = PocoFactory.CreateListFactory(listProp.PropertyType),
                ListGetterSetter = GetterSetter.Create(listProp),
                PrimaryKey = primaryKey,
                ForeignKey = childCol
            };

            assoc.Loader = new OneToManyLoader(assoc);

            _associations.Add(listProp, assoc);

            return assoc;
        }

        public ManyToManyMeta ManyToMany<TParent, TAssoc>(Expression<Func<TParent, IList>> listPropExp)
            where TParent : class, new()
            where TAssoc : class, new()
        {
            EnsureReferences();

            var listProp = listPropExp.GetPropertyInfo();

            var parentType = listProp.DeclaringType;
            var childType = listProp.PropertyType.GetGenericArguments()[0];

            var parentTable = GetTable(parentType);
            var childTable = GetTable(childType);
            var assocTable = this.GetTable<TAssoc>();

            var parentColumn = assocTable.Columns.First(c => c.ReferencedTable == parentTable);
            var childColumn = assocTable.Columns.First(c => c.ReferencedTable == childTable);

            var assoc = new ManyToManyMeta(this)
            {
                ListFactory = PocoFactory.CreateListFactory(listProp.PropertyType),
                ListGetterSetter = GetterSetter.Create(listProp),
                ChildColumn = childColumn,
                ParentColumn = parentColumn
            };

            assoc.Loader = new ManyToManyLoader<TAssoc>(assoc);

            _associations.Add(listProp, assoc);

            return assoc;
        }

        public IDbMeta RegisterModel(Type type)
        {
            PocoDeserializer.RegisterModel(type);
            return this;
        }

        public TableMeta RegisterEntity(Type type)
        {
            var tableMeta = new TableMeta(this)
            {
                Type = type,
                TableName = type.Name,
                Factory = PocoFactory.CreateFactory(type)
            };

            var props = type.GetProperties()
                .Where(p => p.IsColumnProperty())
                .ToList();

            var idProp = props.FirstOrDefault(p => p.Name == "Id");

            tableMeta.AssociationTable = idProp == null;

            var columns = new List<ColumnMeta>();

            foreach (var prop in props)
            {
                var isReference = prop.IsReferenceProperty();

                var columnMeta = new ColumnMeta(this)
                {
                    Table = tableMeta,
                    Property = prop,
                    ColumnName = prop.Name,
                    IsRefrence = isReference,
                    GetterSetter = GetterSetter.Create(prop),
                    ParameterMeta = new ParameterMeta
                    {
                        DbType = isReference ? DbType.Int64 : prop.PropertyType.GetDbType()
                    }
                };

                if (isReference)
                {
                    columnMeta.ColumnName += "Id";
                    columnMeta.Loader = new OneToOneLoader(columnMeta);
                }

                if (tableMeta.AssociationTable)
                {
                    columnMeta.Identity = isReference;
                    columnMeta.AutoIncrement = false;
                }
                else
                {
                    columnMeta.Identity = columnMeta.ColumnName == "Id";
                    columnMeta.AutoIncrement = columnMeta.ColumnName == "Id";
                }

                columns.Add(columnMeta);
            }

            tableMeta.Columns = columns.ToArray();

            if (!tableMeta.AssociationTable)
            {
                tableMeta.IdColumn = columns.First(c => c.ColumnName == "Id");
            }

            foreach (var column in columns)
            {
                _columns.Add(column.Property, column);
            }

            tableMeta.SimpleCommandBuilder = new SimpleCommandBuilder(tableMeta);

            _tables.Add(type, tableMeta);
            
            PocoDeserializer.RegisterEntity(tableMeta);

            EnsureReferences();

            return tableMeta;
        }

        public TableMeta GetTable(Type type)
        {
            return (TableMeta)_tables[type];
        }

        public ColumnMeta GetColumn(PropertyInfo propInf)
        {
            return (ColumnMeta)_columns[propInf];
        }

        public IAssociationMeta GetAssociation(PropertyInfo prop)
        {
            var assoc = _associations[prop] as IAssociationMeta;

            if (assoc == null)
            {
                throw new InvalidOperationException("Association meta not found for " + prop);
            }

            return assoc;
        }

        private void EnsureReferences()
        {
            foreach (var table in _tables.Values.OfType<TableMeta>())
            {
                foreach (var column in table.Columns.Where(c => c.IsRefrence))
                {
                    column.ReferencedTable = GetTable(column.Property.PropertyType);
                }
            }
        }
    }
}