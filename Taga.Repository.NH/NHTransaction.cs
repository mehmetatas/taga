using Taga.Core.Repository;
using INHTransaction = NHibernate.ITransaction;

namespace Taga.Repository.NH
{
    class NHTransaction : ITransaction
    {
        private readonly INHTransaction _transaction;

        public NHTransaction(INHTransaction transaction)
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
    }
}
