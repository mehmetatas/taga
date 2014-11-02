using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Taga.Core.Repository.Mapping
{
    public class TableMapping
    {
        private ColumnMapping[] _columns;
        private ColumnMapping[] _idColumns;
        private ColumnMapping[] _insertColumns;
        private ColumnMapping[] _updateColumns;

        public Type Type { get; internal set; }
        public string TableName { get; internal set; }

        private TableMapping()
        {

        }

        private Dictionary<PropertyInfo, ColumnMapping> _columnMappings;

        public bool HasSingleAutoIncrementId
        {
            get { return IdColumns.Length == 1 && IdColumns[0].IsAutoIncrement; }
        }

        public ColumnMapping[] Columns
        {
            get
            {
                if (_columns == null)
                {
                    _columns = _columnMappings.Values.ToArray();
                }

                return _columns;
            }
        }

        public ColumnMapping[] IdColumns
        {
            get
            {
                if (_idColumns == null)
                {
                    _idColumns = _columnMappings.Values.Where(cm => cm.IsId).ToArray();
                }

                return _idColumns;
            }
        }

        public ColumnMapping[] InsertColumns
        {
            get
            {
                if (_insertColumns == null)
                {
                    _insertColumns = _columnMappings.Values.Where(cm => !cm.IsAutoIncrement).ToArray();
                }
                return _insertColumns;
            }
        }

        public ColumnMapping[] UpdateColumns
        {
            get
            {
                if (_updateColumns == null)
                {
                    _updateColumns = _columnMappings.Values.Where(cm => !cm.IsId).ToArray();
                }
                return _updateColumns;
            }
        }

        internal static TableMapping For(Type type, IDatabaseNamingConvention namingConvention, params string[] idProperties)
        {
            var tableName = namingConvention.GetTableName(type.Name);

            if (idProperties == null || idProperties.Length == 0)
            {
                idProperties = new[] { "Id" };
            }

            var tableMapping = new TableMapping
            {
                Type = type,
                TableName = tableName,
                _columnMappings = new Dictionary<PropertyInfo, ColumnMapping>()
            };

            foreach (var propInf in type.GetProperties())
            {
                if (propInf.PropertyType.IsClass &&
                    propInf.PropertyType != typeof(string) &&
                    propInf.PropertyType != typeof(byte[]))
                {
                    continue;
                }

                var columnName = namingConvention.GetColumnName(propInf.Name);

                var columnMapping = new ColumnMapping
                {
                    ColumnName = columnName,
                    PropertyInfo = propInf
                };

                if (idProperties.Contains(propInf.Name))
                {
                    columnMapping.IsId = true;
                    if (idProperties.Length == 1 && propInf.Name == "Id")
                    {
                        columnMapping.IsAutoIncrement = true;
                    }
                }

                tableMapping._columnMappings.Add(propInf, columnMapping);
            }

            return tableMapping;
        }
    }
}