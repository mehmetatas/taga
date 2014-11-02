using System.Collections.Generic;
using System.Data;
using Taga.Core.IoC;
using Taga.Core.Repository;
using Taga.Core.Repository.Base;
using Taga.Core.Repository.Hybrid;
using Taga.Repository.Hybrid.Commands;

namespace Taga.Repository.Hybrid
{
    public class HybridUnitOfWork : UnitOfWork, IHybridUnitOfWork
    {
        private readonly Queue<IHybridUowCommand> _commands = new Queue<IHybridUowCommand>();

        private IDbConnection _connection;
        private IDbConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    var hybridDbProvider = ServiceProvider.Provider.GetOrCreate<IHybridDbProvider>();
                    _connection = hybridDbProvider.CreateConnection();
                    _connection.Open();
                }
                return _connection;
            }
        }

        private IHybridQueryProvider _queryProvider;
        IHybridQueryProvider IHybridUnitOfWork.QueryProvider
        {
            get
            {
                if (_queryProvider == null)
                {
                    _queryProvider = ServiceProvider.Provider.GetOrCreate<IHybridQueryProvider>();
                    _queryProvider.SetConnection(Connection);
                }
                return _queryProvider;
            }
        }

        IDbCommand IHybridUnitOfWork.CreateCommand()
        {
            var dbCmd = Connection.CreateCommand();

            if (Transaction != null)
            {
                dbCmd.Transaction = ((HybridTransaction)Transaction).DbTransaction;
            }

            return dbCmd;
        }

        protected override ITransaction OnBeginTransaction(IsolationLevel isolationLevel)
        {
            var tran = Connection.BeginTransaction(isolationLevel);
            return new HybridTransaction(tran);
        }

        protected override void OnSave()
        {
            while (_commands.Count > 0)
            {
                var cmd = _commands.Dequeue();

                using (var dbCmd = (this as IHybridUnitOfWork).CreateCommand())
                {
                    cmd.Execute(dbCmd);
                }
            }
        }

        protected override void OnDispose()
        {
            if (_connection == null)
            {
                return;
            }

            _connection.Close();
            _connection.Dispose();
            _connection = null;
        }

        void IHybridUnitOfWork.Insert(object entity)
        {
            _commands.Enqueue(new InsertCommand(entity));
        }

        void IHybridUnitOfWork.Update(object entity)
        {
            _commands.Enqueue(new UpdateCommand(entity));
        }

        void IHybridUnitOfWork.Delete(object entity)
        {
            _commands.Enqueue(new DeleteCommand(entity));
        }
    }
}
