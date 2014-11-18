using System.Data.Common;
using System.Data.Entity;
using Taga.Core.Repository;

namespace Taga.Repository.EF
{
    class EFTransaction : ITransaction
    {
        private readonly DbContextTransaction _transaction;

        public EFTransaction(DbContextTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }

        internal DbTransaction GetDbTransaction()
        {
            return _transaction.UnderlyingTransaction;
        }
    }
}
