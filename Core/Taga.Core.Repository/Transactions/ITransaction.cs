using System;
using System.Data;

namespace Taga.Core.Repository.Transactions
{
    public interface ITransaction : IDisposable
    {
        TransactionState State { get; }

        void Begin(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);
        void Rollback();
        void Commit();
    }
}
