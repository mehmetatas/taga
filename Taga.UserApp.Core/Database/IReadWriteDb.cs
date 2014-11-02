using Taga.Core.Repository;

namespace Taga.UserApp.Core.Database
{
    public interface IReadWriteDb : IReadonlyDb, IUnitOfWork
    {
    }
}