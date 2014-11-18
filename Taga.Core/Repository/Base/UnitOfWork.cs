using System;
using System.Collections.Generic;
using System.Data;
using Taga.Core.Context;

namespace Taga.Core.Repository.Base
{
    public abstract class UnitOfWork : ITransactionalUnitOfWork
    {
        protected ITransaction Transaction { get; private set; }

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
            // _transaction = new Transaction(isolationLevel);
            if (Transaction == null)
            {
                Transaction = OnBeginTransaction(isolationLevel);
            }
        }

        public void RollbackTransaction()
        {
            if (Transaction == null)
            {
                return;
            }

            try
            {
                Transaction.Rollback();
                Transaction.Dispose();
            }
            finally
            {
                Transaction = null;

            }
        }

        public void Save(bool commit)
        {
            OnSave();

            if (Transaction == null || !commit)
            {
                return;
            }

            try
            {
                Transaction.Commit();
                Transaction.Dispose();
            }
            finally
            {
                Transaction = null;
            }
        }

        void IDisposable.Dispose()
        {
            Pop();
            RollbackTransaction();
            OnDispose();
            GC.SuppressFinalize(this);
        }

        protected abstract ITransaction OnBeginTransaction(IsolationLevel isolationLevel);

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

        internal static IUnitOfWork Current
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
            if (Stack.Count > 0)
            {
                throw new NotSupportedException("Nested UnitOfWorks (transactions) are not supported!");
            }
            Stack.Push(uow);
        }

        private static void Pop()
        {
            Stack.Pop();
        }

        #endregion
    }
}