using Taga.Core.Repository;

namespace Taga.UserApp.Core.Database
{
    public interface ITransactionalDb : IReadWriteDb, ITransactionalUnitOfWork
    {
    }
}