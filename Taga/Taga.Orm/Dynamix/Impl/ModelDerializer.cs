using System;
using System.Collections.Generic;
using System.Data;

namespace Taga.Orm.Dynamix.Impl
{
    public class ModelDerializer : IPocoDeserializer
    {
        private readonly Func<object> _factory;
        private readonly IDictionary<string, ISetter> _setters;

        public ModelDerializer(Func<object> factory, IDictionary<string, ISetter> setters)
        {
            _factory = factory;
            _setters = setters;
        }

        public object Deserialize(IDataReader reader)
        {
            var entity = _factory();

            foreach (var setter in _setters)
            {
                var value = reader[setter.Key];
                
                if (value == null || value == DBNull.Value)
                {
                    continue;
                }

                setter.Value.Set(entity, value);
            }

            return entity;
        }
    }
}