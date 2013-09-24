using System.Data;
using Taga.Core.Repository.Base;

namespace Taga.Impl.Repository.EntityFramework
{
    class AdoNetTransaction : Transaction
    {
        private IDbConnection _connection;
        private IDbTransaction _transaction;

        internal AdoNetTransaction(IDbConnection connection)
        {
            _connection = connection;
        }

        protected override void OnBegin(IsolationLevel isolationLevel)
        {
            if (_connection.State != ConnectionState.Open)
                _connection.Open();
            _transaction = _connection.BeginTransaction(isolationLevel);
        }

        protected override void OnCommit()
        {
            _transaction.Commit();
        }

        protected override void OnRollback()
        {
            _transaction.Rollback();
        }

        protected override void OnDispose()
        {
            _transaction.Dispose();
            _connection.Dispose();
            _transaction = null;
            _connection = null;
        }
    }
}
