using Taga.Core.Repository;
using INHTransaction = NHibernate.ITransaction;

namespace Taga.Repository.NH
{
    internal class NHTransaction : ITransaction
    {
        private INHTransaction _transaction;

        public void SetTransaction(INHTransaction transaction)
        {
            _transaction = transaction;
        }

        public void Commit()
        {
            if (_transaction != null)
            {
                _transaction.Commit();
            }
        }

        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
            }
        }

        public void Dispose()
        {
            if (_transaction != null)
            {
                try
                {
                    _transaction.Dispose();
                }
                finally
                {
                    _transaction = null;   
                }
            }
        }
    }
}