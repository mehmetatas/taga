using System;
using System.Collections.Generic;
using System.Data;
using Taga.Orm.Meta;

namespace Taga.Orm.Dynamix.Impl
{
    public class EntityDeserializer : IPocoDeserializer
    {
        private readonly Func<object> _factory;
        private readonly IDictionary<string, IEnumerable<ColumnMeta>> _propertyChain;

        public EntityDeserializer(Func<object> factory, IDictionary<string, IEnumerable<ColumnMeta>> propertyChain)
        {
            _factory = factory;
            _propertyChain = propertyChain;
        }

        public object Deserialize(IDataReader reader)
        {
            var entity = _factory();

            foreach (var kv in _propertyChain)
            {
                var value = reader[kv.Key];

                if (value == null || value == DBNull.Value)
                {
                    continue;
                }

                var chain = kv.Value;

                ColumnMeta lastProp = null;
                var tmp = entity;
                foreach (var columnMeta in chain)
                {
                    lastProp = columnMeta;

                    if (!columnMeta.IsRefrence)
                    {
                        continue;
                    }

                    var refObj = columnMeta.GetterSetter.Get(tmp);

                    if (refObj == null)
                    {
                        refObj = columnMeta.ReferencedTable.Factory();
                        columnMeta.GetterSetter.Set(tmp, refObj);
                    }

                    tmp = refObj;
                }

                if (lastProp.IsRefrence)
                {
                    lastProp.ReferencedTable.IdColumn.GetterSetter.Set(tmp, value);
                }
                else
                {
                    lastProp.GetterSetter.Set(tmp, value);
                }
            }

            return entity;
        }
    }
}