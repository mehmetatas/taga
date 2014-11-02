using Taga.Core.IoC;
using Taga.Core.Repository;

namespace Taga.UserApp.Core.Database
{
    public static class Db
    {
        private static ITransactionalUnitOfWork CreateUnitOfWork()
        {
            return ServiceProvider.Provider.GetOrCreate<ITransactionalUnitOfWork>();
        }

        public static IReadonlyDb Readonly()
        {
            return new ReadonlyDb(CreateUnitOfWork());
        }

        public static IReadWriteDb ReadWrite()
        {
            return new ReadWriteDb(CreateUnitOfWork());
        }

        public static ITransactionalDb Transactional()
        {
            return new TransactionalReadWriteDb(CreateUnitOfWork());
        }
    }
}
