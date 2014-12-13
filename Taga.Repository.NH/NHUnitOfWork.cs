using NHibernate;
using System.Data;
using Taga.Core.Repository.Base;

namespace Taga.Repository.NH
{
    public class NHUnitOfWork : UnitOfWork, INHUnitOfWork
    {
        private readonly ISessionFactory _sessionFactory;
        private IStatelessSession _session;
        private bool _beginTransactionOnSessionStart;
        private IsolationLevel _transactionIsolationLevel;
        private NHTransaction _transaction;

        public IStatelessSession Session
        {
            get
            {
                if (_session == null)
                {
                    _session = _sessionFactory.OpenStatelessSession();
                    if (_beginTransactionOnSessionStart)
                    {
                        BeginTransaction();
                    }
                }
                return _session;
            }
        }


        public NHUnitOfWork(ISessionFactory sessionFactory)
        {
            _sessionFactory = sessionFactory;
        }

        IStatelessSession INHUnitOfWork.Session
        {
            get { return Session; }
        }

        protected override void OnSave()
        {
            Session.GetSessionImplementation().Flush();
        }

        protected override void OnDispose()
        {
            if (_session != null)
            {
                _session.Dispose();   
            }
        }

        protected override Core.Repository.ITransaction OnBeginTransaction(IsolationLevel isolationLevel)
        {
            if (_transaction != null)
            {
                return _transaction;
            }

            _transaction = new NHTransaction();

            _transactionIsolationLevel = isolationLevel;

            if (_session == null)
            {
                _beginTransactionOnSessionStart = true;
            }
            else
            {
                BeginTransaction();
            }

            return _transaction;
        }

        private void BeginTransaction()
        {
            _session.BeginTransaction(_transactionIsolationLevel);

            _transaction.SetTransaction(_session.Transaction);
        }
    }
}