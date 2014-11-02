
namespace Taga.Core.Repository.Mapping
{
    public class MappingProvider : IMappingProvider
    {
        private DatabaseMapping _databaseMapping;

        public void SetDatabaseMapping(DatabaseMapping databaseMapping)
        {
            _databaseMapping = databaseMapping;
        }

        public DatabaseMapping GetDatabaseMapping()
        {
            return _databaseMapping;
        }
    }
}