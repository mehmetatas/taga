using System.Data;

namespace Taga.Core.Repository
{
    public interface ITransactionalUnitOfWork : IUnitOfWork
    {
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        void Save(bool commit);
    }
}