using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Taga.Orm.Meta;

namespace Taga.Orm.Dynamix.Impl
{
    public static class PocoDeserializer
    {
        private static readonly Hashtable DefaultDeserializers = new Hashtable();

        public static IPocoDeserializer GetDefault<T>() where T : class,new()
        {
            return GetDefault(typeof(T));
        }

        public static void RegisterEntity(TableMeta tableMeta)
        {
            var propChain = tableMeta.Columns.ToDictionary(c => c.ColumnName, c => (IEnumerable<ColumnMeta>)new[] { c });

            var deserializer = new EntityDeserializer(tableMeta.Factory, propChain);

            DefaultDeserializers.Add(tableMeta.Type, deserializer);
        }

        public static void RegisterModel<T>() where T : class,new()
        {
            RegisterModel(typeof(T));
        }

        public static void RegisterModel(Type type)
        {
            var setters = type.GetProperties()
                .ToDictionary(prop => prop.Name, prop => (ISetter) GetterSetter.Create(prop));
            
            var factory = PocoFactory.CreateFactory(type);
            
            var deserializer = new ModelDerializer(factory, setters);
            
            DefaultDeserializers.Add(type, deserializer);
        }

        public static IPocoDeserializer GetDefault(Type type)
        {
            return (IPocoDeserializer)DefaultDeserializers[type];
        }
    }
}