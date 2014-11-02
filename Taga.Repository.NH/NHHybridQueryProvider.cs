using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Taga.Core.Repository.Hybrid;

namespace Taga.Repository.NH
{
    public class NHHybridQueryProvider : IHybridQueryProvider
    {
        private ISession _session;
        private readonly ISessionFactory _sessionFactory;
        private readonly INHSpCallBuilder _spCallBuilder;

        public NHHybridQueryProvider(ISessionFactory sessionFactory, INHSpCallBuilder spCallBuilder)
        {
            _sessionFactory = sessionFactory;
            _spCallBuilder = spCallBuilder;
        }

        public void SetConnection(IDbConnection connection)
        {
            _session = _sessionFactory.OpenSession(connection);
        }

        public IQueryable<T> Select<T>() where T : class
        {
            return _session.Query<T>();
        }

        public IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false) where T : class
        {
            return NHRepository.ExecuteQuery<T>(_session, _spCallBuilder, spNameOrSql, args, rawSql);
        }
    }
}
