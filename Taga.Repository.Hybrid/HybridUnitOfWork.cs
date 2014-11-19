using System;
using System.Collections.Generic;
using System.Data;
using Taga.Core.Repository;
using Taga.Core.Repository.Command;
using Taga.Core.Repository.Hybrid;
using Taga.Repository.Hybrid.Commands;

namespace Taga.Repository.Hybrid
{
    public class HybridUnitOfWork : ITransactionalUnitOfWork
    {
        [ThreadStatic]
        internal static HybridUnitOfWork Current;

        private readonly Queue<IHybridUowCommand> _commands = new Queue<IHybridUowCommand>();

        private readonly IHybridAdapter _adapter;

        public HybridUnitOfWork(IHybridAdapter adapter)
        {
            _adapter = adapter;
            Current = this;
        }

        public IHybridAdapter Adapter
        {
            get { return _adapter; }
        }

        internal void Insert(object entity)
        {
            _commands.Enqueue(new InsertCommand(entity));
        }

        internal void Update(object entity)
        {
            _commands.Enqueue(new UpdateCommand(entity));
        }

        internal void Delete(object entity)
        {
            _commands.Enqueue(new DeleteCommand(entity));
        }

        internal void NonQuery(ICommand command)
        {
            _commands.Enqueue(new NonQueryCommand(command));
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _adapter.UnitOfWork.BeginTransaction(isolationLevel);
        }

        public void Save(bool commit)
        {
            DoSave();
            _adapter.UnitOfWork.Save(commit);
        }

        public void Save()
        {
            DoSave();
            _adapter.UnitOfWork.Save();
        }

        private void DoSave()
        {
            while (_commands.Count > 0)
            {
                var cmd = _commands.Dequeue();

                using (var dbCmd = _adapter.CreateCommand())
                {
                    cmd.Execute(dbCmd);
                }
            }
        }

        public void Dispose()
        {
            _adapter.UnitOfWork.Dispose();
        }
    }
}
