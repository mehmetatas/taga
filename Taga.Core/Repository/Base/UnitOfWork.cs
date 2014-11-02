using System;
using System.Collections.Generic;
using System.Data;
using Taga.Core.Context;

namespace Taga.Core.Repository.Base
{
    public abstract class UnitOfWork : ITransactionalUnitOfWork
    {
        private ITransaction _transaction;

        protected UnitOfWork()
        {
            Push(this);
        }

        public void Save()
        {
            Save(false);
        }

        public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            _transaction = new Transaction(isolationLevel);
        }

        public void RollbackTransaction()
        {
            if (_transaction == null)
                return;

            _transaction.Rollback();
            _transaction.Dispose();
            _transaction = null;
        }

        public void Save(bool commit)
        {
            OnSave();

            if (_transaction == null || !commit)
                return;

            _transaction.Commit();
            _transaction.Dispose();
            _transaction = null;
        }

        void IDisposable.Dispose()
        {
            Pop();
            RollbackTransaction();
            OnDispose();
        }

        protected abstract void OnSave();

        protected virtual void OnDispose()
        {
        }

        #region Stack

        private static Stack<IUnitOfWork> Stack
        {
            get
            {
                var uowStack = CallContext.Current.Get<Stack<IUnitOfWork>>("UnitOfWorkStack");
                if (uowStack == null)
                {
                    uowStack = new Stack<IUnitOfWork>();
                    CallContext.Current["UnitOfWorkStack"] = uowStack;
                }
                return uowStack;
            }
        }

        public static IUnitOfWork Current
        {
            get
            {
                if (Stack.Count == 0)
                {
                    throw new InvalidOperationException("No UnitOfWork is available!");
                }
                return Stack.Peek();
            }
        }

        private static void Push(IUnitOfWork uow)
        {
            Stack.Push(uow);
        }

        private static void Pop()
        {
            Stack.Pop();
        }

        #endregion
    }
}