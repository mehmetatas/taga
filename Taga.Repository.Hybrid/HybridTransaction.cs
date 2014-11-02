using System.Data;
using Taga.Core.Repository;

namespace Taga.Repository.Hybrid
{
    class HybridTransaction : ITransaction
    {
        internal readonly IDbTransaction DbTransaction;

        public HybridTransaction(IDbTransaction transaction)
        {
            DbTransaction = transaction;
        }

        public void Commit()
        {
            DbTransaction.Commit();
        }

        public void Rollback()
        {
            DbTransaction.Rollback();
        }

        public void Dispose()
        {
            DbTransaction.Dispose();
        }
    }
}