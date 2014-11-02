using System.Data;
using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using Taga.Core.Repository;
using Taga.Core.Repository.Base;

namespace Taga.Repository.NH
{
    public class NHRepository : IRepository
    {
        private readonly ISession _session;
        private readonly INHSpCallBuilder _spCallBuilder;

        public NHRepository(INHSpCallBuilder spCallBuilder)
        {
            var uow = (INHUnitOfWork)UnitOfWork.Current;
            _session = uow.Session;
            _spCallBuilder = spCallBuilder;
        }

        public void Insert<T>(T entity) where T : class
        {
            _session.Save(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _session.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            //if (!_session.Contains(entity))
            //{
            //    // Just to attach entity. Otherwise NHibernate may try to select entity before delete
            //    _session.Update(entity);
            //}
            _session.Delete(entity);
        }

        public IQueryable<T> Select<T>() where T : class
        {
            return _session.Query<T>();
        }

        public IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
            where T : class
        {
            return ExecuteQuery<T>(_session, _spCallBuilder, spNameOrSql, args, rawSql);
        }

        public void NonQuery(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            var query = BuildSql(_session, _spCallBuilder, spNameOrSql, args, rawSql);

            query.ExecuteUpdate();
        }

        internal static IList<T> ExecuteQuery<T>(ISession session, INHSpCallBuilder spCallBuilder, string spNameOrSql,
            IDictionary<string, object> args = null, bool rawSql = false)
            where T : class
        {
            var query = BuildSql(session, spCallBuilder, spNameOrSql, args, rawSql);

            return query.AddEntity(typeof(T)).List<T>();
        }

        private static ISQLQuery BuildSql(ISession session, INHSpCallBuilder spCallBuilder, string spNameOrSql,
            IDictionary<string, object> args, bool rawSql)
        {
            var command = rawSql
                ? spNameOrSql
                : spCallBuilder.BuildSpCall(spNameOrSql, args);

            var query = session.CreateSQLQuery(command);

            if (args != null)
            {
                foreach (var arg in args)
                {
                    query.SetParameter(arg.Key, arg.Value);
                }
            }
            return query;
        }
    }
}