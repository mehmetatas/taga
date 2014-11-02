using System.Data;

namespace Taga.Core.Repository
{
    public interface ITransactionalUnitOfWork : IUnitOfWork
    {
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        void RollbackTransaction();

        void Save(bool commit);
    }
}