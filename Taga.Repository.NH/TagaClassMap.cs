using FluentNHibernate.Mapping;
using System;
using System.Linq.Expressions;
using Taga.Core.IoC;
using Taga.Core.Repository;
using Taga.Core.Repository.Mapping;

namespace Taga.Repository.NH
{
    public abstract class TagaClassMap<T> : ClassMap<T> where T : class
    {
        protected TagaClassMap()
        {
            var mappingProv = ServiceProvider.Provider.GetOrCreate<IMappingProvider>();

            var dbSystem = mappingProv.GetDatabaseMapping().DbSystem;

            var tableMapping = mappingProv.GetTableMapping<T>();

            Table(tableMapping.TableName);

            CompositeIdentityPart<T> compositeIdPart = null;

            var hasCompositeKey = tableMapping.IdColumns.Length > 1;

            foreach (var columnMapping in tableMapping.Columns)
            {
                var propInf = columnMapping.PropertyInfo;

                var parameter = Expression.Parameter(tableMapping.Type, "entity");
                var property = Expression.Property(parameter, propInf);
                var body = Expression.Convert(property, typeof(object));
                var funcType = typeof(Func<T, object>);
                var lambda = (Expression<Func<T, object>>)Expression.Lambda(funcType, body, parameter);

                var columnName = columnMapping.ColumnName;

                if (columnMapping.IsId)
                {
                    if (hasCompositeKey)
                    {
                        if (compositeIdPart == null)
                        {
                            compositeIdPart = CompositeId();
                        }

                        compositeIdPart.KeyProperty(lambda, columnName);
                    }
                    else
                    {
                        var idPart = Id(lambda, columnName);

                        if (columnMapping.IsAutoIncrement)
                        {
                            if (dbSystem == DbSystem.Oracle)
                            {
                                idPart.GeneratedBy.SequenceIdentity("SEQ_" + tableMapping.TableName);
                            }
                            else
                            {
                                idPart.GeneratedBy.Identity();   
                            }
                        }
                        else
                        {
                            idPart.GeneratedBy.Assigned();
                        }
                    }
                }
                else
                {
                    var propPart = Map(lambda, columnName);

                    if (propInf.PropertyType.IsEnum)
                    {
                        propPart.CustomType(propInf.PropertyType);
                    }
                }
            }
        } 
    }
}