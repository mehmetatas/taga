using NHibernate;
using System.Data;
using Taga.Core.Repository.Base;
using ITransaction = Taga.Core.Repository.ITransaction;

namespace Taga.Repository.NH
{
    public class NHUnitOfWork : UnitOfWork, INHUnitOfWork
    {
        private readonly ISession _session;

        public NHUnitOfWork(ISessionFactory sessionFactory)
        {
            _session = sessionFactory.OpenSession();
            _session.FlushMode = FlushMode.Auto;
        }

        ISession INHUnitOfWork.Session
        {
            get { return _session; }
        }

        protected override ITransaction OnBeginTransaction(IsolationLevel isolationLevel)
        {
            var tran = _session.BeginTransaction(isolationLevel);
            return new NHTransaction(tran);
        }

        protected override void OnSave()
        {
            _session.Flush();
        }

        protected override void OnDispose()
        {
            _session.Dispose();
        }
    }
}