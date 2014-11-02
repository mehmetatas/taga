using System.Data;
using Taga.Core.Repository.Transactions;

namespace Taga.Core.Repository.Base
{
    public abstract class Transaction : ITransaction
    {
        public TransactionState State { get; private set; }

        protected Transaction()
        {
            State = TransactionState.None;
        }

        public void Begin(IsolationLevel isolationLevel)
        {
            if (State == TransactionState.Started)
                throw new TransactionAlreadyStartedException();

            if (State == TransactionState.Complete)
                throw new TransactionAlreadyCompletedException();

            OnBegin(isolationLevel);

            State = TransactionState.Started;
        }

        public void Commit()
        {
            Complete(false);
        }

        public void Rollback()
        {
            Complete(true);
        }

        public void Dispose()
        {
            if (State == TransactionState.None)
                return;

            if (State == TransactionState.Started)
                Rollback();

            OnDispose();
        }

        protected virtual void OnDispose()
        {

        }

        private void Complete(bool rollback)
        {
            if (State == TransactionState.None)
                throw new TransactionNotStartedException();

            if (State == TransactionState.Complete)
                throw new TransactionAlreadyCompletedException();

            try
            {
                if (rollback)
                    OnRollback();
                else
                    OnCommit();
            }
            finally
            {
                State = TransactionState.Complete;
            }
        }

        protected abstract void OnBegin(IsolationLevel isolationLevel);
        protected abstract void OnCommit();
        protected abstract void OnRollback();
    }
}
