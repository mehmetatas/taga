using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Taga.Core.Repository.Mapping.NamingConvention;

namespace Taga.Core.Repository.Mapping
{
    public class DatabaseMapping
    {
        private readonly DbSystem _dbSystem;
        private readonly Dictionary<Type, TableMapping> _tableMappings;

        private IDatabaseNamingConvention _namingConvention;

        private TableMapping _currentTableMapping;

        private DatabaseMapping(DbSystem dbSystem)
        {
            _dbSystem = dbSystem;
            _tableMappings = new Dictionary<Type, TableMapping>();
            _namingConvention = CreateNamingConvention(dbSystem);
        }

        public DbSystem DbSystem
        {
            get { return _dbSystem; }
        }

        public DatabaseMapping Map(Type entityType, params string[] idProperties)
        {
            _currentTableMapping = TableMapping.For(entityType, _namingConvention, idProperties);
            _tableMappings.Add(entityType, _currentTableMapping);
            return this;
        }

        public DatabaseMapping ToTable(string tableName)
        {
            _currentTableMapping.TableName = tableName;
            return this;
        }

        public IEnumerable<TableMapping> TableMappings
        {
            get { return _tableMappings.Values; }
        } 

        public TableMapping this[Type entityType]
        {
            get
            {
                var type = entityType;
                while (type != typeof(object) && type != null)
                {
                    if (_tableMappings.ContainsKey(type))
                    {
                        return _tableMappings[type];        
                    }
                    type = type.BaseType;
                }
                throw new InvalidOperationException("Unknown entity type: " + entityType);
            }
        }

        public DatabaseMapping WithNamingConvention(IDatabaseNamingConvention namingConvention)
        {
            _namingConvention = namingConvention;
            return this;
        }

        public static DatabaseMapping For(DbSystem dbSystem)
        {
            return new DatabaseMapping(dbSystem);
        }

        private static IDatabaseNamingConvention CreateNamingConvention(DbSystem dbSystem)
        {
            switch (dbSystem)
            {
                case DbSystem.SqlServer:
                    return new SqlServerNamingConvention(true);
                case DbSystem.MySql:
                    return new MySqlNamingConvention(true);
                case DbSystem.Oracle:
                    return new OracleNamingConvention(true);
                default:
                    throw new ArgumentOutOfRangeException("dbSystem");
            }
        }
    }

    public static class DatabaseMappingExtensions
    {
        public static DatabaseMapping Map<T>(this DatabaseMapping databaseMapping, params Expression<Func<T, object>>[] propExpressions)
        {
            var propNames = new List<string>();
            
            foreach (var propExpression in propExpressions)
            {
                MemberExpression memberExpression;
                if (propExpression.Body is UnaryExpression)
                {
                    var unaryExp = (UnaryExpression) propExpression.Body;
                    memberExpression = (MemberExpression) unaryExp.Operand;
                }
                else
                {
                    memberExpression = (MemberExpression) propExpression.Body;
                }

                var propInf = (PropertyInfo) memberExpression.Member;

                propNames.Add(propInf.Name);
            }

            return databaseMapping.Map(typeof(T), propNames.ToArray());
        }
    }
}