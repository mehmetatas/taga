using System.Data;
using System.Data.Entity;
using Taga.Core.Repository;
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

        protected override ITransaction OnBeginTransaction(IsolationLevel isolationLevel)
        {
            return new EFTransaction(_dbContext.Database.BeginTransaction(isolationLevel));
        }

        internal IDbCommand CreateDbCommand()
        {
            var conn = _dbContext.Database.Connection;

            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            var cmd = conn.CreateCommand();
            var tran = Transaction as EFTransaction;

            if (tran != null)
            {
                cmd.Transaction = tran.GetDbTransaction();
            }

            return cmd;
        }
    }
}
