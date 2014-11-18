using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Taga.Core.Repository;
using Taga.Core.Repository.Hybrid;

namespace Taga.Repository.EF
{
    public class EFHybridAdapter : IHybridAdapter
    {
        private readonly DbContext _dbContext;
        private readonly EFUnitOfWork _uow;

        public EFHybridAdapter(DbContext dbContext)
        {
            _dbContext = dbContext;
            _uow = new EFUnitOfWork(dbContext);
        }

        ITransactionalUnitOfWork IHybridAdapter.UnitOfWork
        {
            get { return _uow; }
        }

        IDbCommand IHybridAdapter.CreateCommand()
        {
            return _uow.CreateDbCommand();
        }

        IQueryable<T> IHybridAdapter.Select<T>()
        {
            return _dbContext.Set<T>();
        }

        IList<T> IHybridAdapter.Query<T>(string spNameOrSql, IDictionary<string, object> args, bool rawSql)
        {
            return EFRepository.ExecuteQuery<T>(_dbContext, spNameOrSql, args, rawSql);
        }
    }
}
