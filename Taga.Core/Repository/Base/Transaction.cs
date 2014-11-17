using System;
using System.Transactions;

namespace Taga.Core.Repository.Base
{
    class Transaction : ITransaction
    {
        private readonly TransactionScope _transaction;

        public Transaction(System.Data.IsolationLevel isolationLevel)
        {
            IsolationLevel tranIsoLevel;
            switch (isolationLevel)
            {
                case System.Data.IsolationLevel.ReadCommitted:
                    tranIsoLevel = IsolationLevel.ReadCommitted;
                    break;
                case System.Data.IsolationLevel.ReadUncommitted:
                    tranIsoLevel = IsolationLevel.ReadUncommitted;
                    break;
                default:
                    throw new NotSupportedException("Unsupported IsolationLevel: " + isolationLevel);
            }

            _transaction = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions { IsolationLevel = tranIsoLevel });
        }

        public void Commit()
        {
            _transaction.Complete();
        }

        public void Rollback()
        {

        }

        public void Dispose()
        {
            _transaction.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
