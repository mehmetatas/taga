using System;
using System.Data;

namespace Taga.Orm.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted);

        void Rollback();

        void Commit();
    }
}
