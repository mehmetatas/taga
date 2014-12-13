using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Taga.Core.Repository;
using Taga.Core.Repository.Hybrid;

namespace Taga.Repository.NH
{
    public class NHHybridAdapter : IHybridAdapter
    {
        private readonly NHUnitOfWork _uow;
        private readonly INHSpCallBuilder _spCallBuilder;

        public NHHybridAdapter(ISessionFactory sessionFactory, INHSpCallBuilder spCallBuilder)
        {
            _uow = new NHUnitOfWork(sessionFactory);
            _spCallBuilder = spCallBuilder;
        }

        ITransactionalUnitOfWork IHybridAdapter.UnitOfWork
        {
            get { return _uow; }
        }

        IDbCommand IHybridAdapter.CreateCommand()
        {
            var cmd = _uow.Session.Connection.CreateCommand();
            _uow.Session.Transaction.Enlist(cmd);
            return cmd;
        }

        IQueryable<T> IHybridAdapter.Select<T>()
        {
            return _uow.Session.Query<T>();
        }

        IList<T> IHybridAdapter.Query<T>(string spNameOrSql, IDictionary<string, object> args, bool rawSql)
        {
            return NHRepository.ExecuteQuery<T>(_uow.Session, _spCallBuilder, spNameOrSql, args, rawSql);
        }
    }
}
