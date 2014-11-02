using NHibernate;
using Taga.Core.Repository.Base;

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