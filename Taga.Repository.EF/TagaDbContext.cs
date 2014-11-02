using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.Data.Entity.ModelConfiguration.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
using Taga.Core.Dynamix;
using Taga.Core.IoC;
using Taga.Core.Mail;
using Taga.Core.Repository.Mapping;

namespace Taga.Repository.EF
{
    public abstract class TagaDbContext : DbContext
    {
        static TagaDbContext()
        {
            Database.SetInitializer<TagaDbContext>(null);
        }

        protected TagaDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {

        }

        protected TagaDbContext(DbConnection connection)
            : base(connection, false)
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var mappingProv = ServiceProvider.Provider.GetOrCreate<IMappingProvider>();

            var dbMapping = mappingProv.GetDatabaseMapping();

            var builder = new DbModelBuilderWrapper(modelBuilder);

            foreach (var tableMapping in dbMapping.TableMappings)
            {
                builder.Entity(tableMapping.Type)
                    .ToTable(tableMapping.TableName);

                foreach (var columnMapping in tableMapping.Columns)
                {
                    builder.Property(columnMapping.PropertyInfo, columnMapping.ColumnName);
                }

                if (tableMapping.IdColumns.Length == 1)
                {
                    var keyProp = tableMapping.IdColumns[0];
                    builder.HasKey(keyProp.PropertyInfo);
                }
                else
                {
                    var keyProps = tableMapping.IdColumns.Select(k => k.PropertyInfo).ToArray();
                    builder.HasCompositeKey(keyProps);
                }

                var mapping = tableMapping;
                var ignoredProps = tableMapping.Type.GetProperties()
                    .Where(prop => mapping.Columns.All(col => col.PropertyInfo != prop));

                foreach (var ignoredProp in ignoredProps)
                {
                    builder.Ignore(ignoredProp);
                }
            }
        }
    }

    class DbModelBuilderWrapper
    {
        private readonly DbModelBuilder _modelBuilder;

        private readonly MethodInfo _entityMethod;

        private Type _entityType;
        private object _entityTypeConfig;

        public DbModelBuilderWrapper(DbModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;

            var modelBuilderType = typeof (DbModelBuilder);

            _entityMethod = modelBuilderType.GetMethod("Entity");
        }

        public DbModelBuilderWrapper Entity(Type type)
        {
            _entityType = type;
            
            _entityTypeConfig = _entityMethod
                .MakeGenericMethod(type)
                .Invoke(_modelBuilder, null);

            return this;
        }

        public DbModelBuilderWrapper ToTable(string tableName)
        {
            var toTableMethod = _entityTypeConfig.GetType().GetMethod("ToTable", new[] { typeof(string) });

            toTableMethod
                .Invoke(_entityTypeConfig, new object[] {tableName});

            return this;
        }

        public DbModelBuilderWrapper Property(PropertyInfo propertyInfo, string columnName)
        {
            var parameter = Expression.Parameter(_entityType, "entity");
            var body = Expression.Property(parameter, propertyInfo);
            var funcType = typeof(Func<,>).MakeGenericType(_entityType, propertyInfo.PropertyType);
            var lambda = Expression.Lambda(funcType, body, parameter);

            MethodInfo propertyMethod;

            if (propertyInfo.PropertyType == typeof (byte[]) ||
                propertyInfo.PropertyType == typeof (decimal) ||
                propertyInfo.PropertyType == typeof (string) ||
                propertyInfo.PropertyType == typeof (DateTime))
            {
                var argExpressionType = typeof (Expression<>).MakeGenericType(funcType);

                propertyMethod = _entityTypeConfig.GetType().GetMethod("Property", new[] {argExpressionType});
            }
            else
            {
                var argFuncType = typeof(Func<,>).MakeGenericType(_entityType, typeof(Refl.T1));
                var argExpressionType = typeof(Expression<>).MakeGenericType(argFuncType);

                propertyMethod = GetMethod(_entityTypeConfig.GetType(), "Property", argExpressionType)
                    .MakeGenericMethod(propertyInfo.PropertyType);
            } 
            
            var propConfig = (PrimitivePropertyConfiguration)propertyMethod
                     .Invoke(_entityTypeConfig, new object[] { lambda });

            propConfig.HasColumnName(columnName);

            return this;
        }

        public DbModelBuilderWrapper HasKey(PropertyInfo propertyInfo)
        {
            var parameter = Expression.Parameter(_entityType, "entity");
            var body = Expression.Property(parameter, propertyInfo);
            var funcType = typeof(Func<,>).MakeGenericType(_entityType, propertyInfo.PropertyType);
            var lambda = Expression.Lambda(funcType, body, parameter);

            _entityTypeConfig.GetType().GetMethod("HasKey")
                .MakeGenericMethod(propertyInfo.PropertyType)
                .Invoke(_entityTypeConfig, new object[] {lambda});

            return this;
        }

        public DbModelBuilderWrapper Ignore(PropertyInfo propertyInfo)
        {
            var parameter = Expression.Parameter(_entityType, "entity");
            var body = Expression.Property(parameter, propertyInfo);
            var funcType = typeof(Func<,>).MakeGenericType(_entityType, propertyInfo.PropertyType);
            var lambda = Expression.Lambda(funcType, body, parameter);

            _entityTypeConfig.GetType().GetMethod("Ignore")
              .MakeGenericMethod(propertyInfo.PropertyType)
              .Invoke(_entityTypeConfig, new object[] { lambda });

            return this;
        }

        public DbModelBuilderWrapper HasCompositeKey(params PropertyInfo[] keyProps)
        {
            var dic = keyProps.ToDictionary(p => p.Name, p => p.PropertyType);

            var pocoType = PocoBuilder.BuildPoco(dic);
            
            var parameter = Expression.Parameter(_entityType, "entity");

            var args = dic.Select(kv => Expression.Property(parameter, kv.Key)).Cast<Expression>().ToArray();

            var body = Expression.New(pocoType.GetConstructor(keyProps.Select(p => p.PropertyType).ToArray()), args, pocoType.GetProperties());
            var funcType = typeof(Func<,>).MakeGenericType(_entityType, pocoType);
            var lambda = Expression.Lambda(funcType, body, parameter);

            _entityTypeConfig.GetType().GetMethod("HasKey")
                .MakeGenericMethod(pocoType)
                .Invoke(_entityTypeConfig, new object[] { lambda });

            return this;
        }

        static class Refl
        {
            public sealed class T1 { }
            public sealed class T2 { }
            public sealed class T3 { }
            // ... more, if you so desire.
        }

        public static MethodInfo GetMethod(Type t, string name, params Type[] parameters)
        {
            foreach (var method in t.GetMethods())
            {
                // easiest case: the name doesn't match!
                if (method.Name != name)
                    continue;
                // set a flag here, which will eventually be false if the method isn't a match.
                var correct = true;
                if (method.IsGenericMethodDefinition)
                {
                    // map the "private" Type objects which are the type parameters to
                    // my public "Tx" classes...
                    var d = new Dictionary<Type, Type>();
                    var args = method.GetGenericArguments();
                    if (args.Length >= 1)
                        d[typeof(Refl.T1)] = args[0];
                    if (args.Length >= 2)
                        d[typeof(Refl.T2)] = args[1];
                    if (args.Length >= 3)
                        d[typeof(Refl.T3)] = args[2];
                    if (args.Length > 3)
                        throw new NotSupportedException("Too many type parameters.");

                    var p = method.GetParameters();
                    for (var i = 0; i < p.Length; i++)
                    {
                        // Find the Refl.TX classes and replace them with the 
                        // actual type parameters.
                        var pt = Substitute(parameters[i], d);
                        // Then it's a simple equality check on two Type instances.
                        if (pt != p[i].ParameterType)
                        {
                            correct = false;
                            break;
                        }
                    }
                    if (correct)
                        return method;
                }
                else
                {
                    var p = method.GetParameters();
                    for (var i = 0; i < p.Length; i++)
                    {
                        var pt = parameters[i];
                        if (pt != p[i].ParameterType)
                        {
                            correct = false;
                            break;
                        }
                    }
                    if (correct)
                        return method;
                }
            }
            return null;
        }

        private static Type Substitute(Type t, IDictionary<Type, Type> env)
        {
            // We only really do something if the type 
            // passed in is a (constructed) generic type.
            if (t.IsGenericType)
            {
                var targs = t.GetGenericArguments();
                for (int i = 0; i < targs.Length; i++)
                    targs[i] = Substitute(targs[i], env); // recursive call
                t = t.GetGenericTypeDefinition();
                t = t.MakeGenericType(targs);
            }
            // see if the type is in the environment and sub if it is.
            return env.ContainsKey(t) ? env[t] : t;
        }
    }
}
