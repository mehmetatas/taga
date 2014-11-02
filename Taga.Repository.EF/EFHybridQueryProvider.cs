using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using Taga.Core.Repository;
using Taga.Core.Repository.Hybrid;

namespace Taga.Repository.EF
{
    public abstract class EFHybridQueryProvider : IHybridQueryProvider
    {
        private DbContext _dbContext;

        public void SetConnection(IDbConnection connection)
        {
            _dbContext = CreateDbContext((DbConnection)connection);
        }

        public IQueryable<T> Select<T>() where T : class
        {
            return _dbContext.Set<T>();
        }

        public IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false) where T : class
        {
            return EFRepository.ExecuteQuery<T>(_dbContext, spNameOrSql, args, rawSql);
        }

        protected abstract DbContext CreateDbContext(DbConnection connection);
    }
}
