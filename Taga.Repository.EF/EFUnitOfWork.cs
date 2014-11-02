using System.Data.Entity;
using Taga.Core.Repository.Base;

namespace Taga.Repository.EF
{
    public class EFUnitOfWork : UnitOfWork, IEFUnitOfWork
    {
        private readonly DbContext _dbContext;

        public EFUnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        DbContext IEFUnitOfWork.DbContext
        {
            get { return _dbContext; }
        }

        protected override void OnSave()
        {
            _dbContext.SaveChanges();
        }

        protected override void OnDispose()
        {
            _dbContext.Dispose();
        }
    }
}
