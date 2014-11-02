using System;

namespace Taga.Core.Repository.Mapping
{
    public interface IMappingProvider
    {
        void SetDatabaseMapping(DatabaseMapping mappings);

        DatabaseMapping GetDatabaseMapping();
    }

    public static class MappingProviderExtensions
    {
        public static TableMapping GetTableMapping<T>(this IMappingProvider prov) where T : class
        {
            return prov.GetTableMapping(typeof(T));
        }

        public static TableMapping GetTableMapping(this IMappingProvider prov, Type entityType)
        {
            return prov.GetDatabaseMapping()[entityType];
        }
    }
}
