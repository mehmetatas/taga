using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using Taga.Core.Repository;
using Taga.Core.Repository.Base;
using Taga.Core.Repository.Command;
using Taga.Core.Repository.Command.Builders;

namespace Taga.Repository.NH
{
    public class NHRepository : IRepository
    {
        private static readonly ICommandBuilder CommandBuilder = new NHibernateCommandBuilder();

        private readonly ISession _session;
        private readonly INHSpCallBuilder _spCallBuilder;

        public NHRepository(INHSpCallBuilder spCallBuilder)
        {
            var uow = (INHUnitOfWork)UnitOfWork.Current;
            _session = uow.Session;
            _spCallBuilder = spCallBuilder;
        }

        public void Insert<T>(T entity) where T : class, new()
        {
            _session.Save(entity);
        }

        public void Update<T>(T entity) where T : class, new()
        {
            _session.Update(entity);
        }

        public void Delete<T>(T entity) where T : class, new()
        {
            _session.Delete(entity);
        }

        public IQueryable<T> Select<T>() where T : class, new()
        {
            return _session.Query<T>();
        }

        public IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
            where T : class, new()
        {
            return ExecuteQuery<T>(_session, _spCallBuilder, spNameOrSql, args, rawSql);
        }

        public void NonQuery(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            ExecuteNonQuery(_session, _spCallBuilder, spNameOrSql, args, rawSql);
        }

        internal static IList<T> ExecuteQuery<T>(ISession session, INHSpCallBuilder spCallBuilder, string spNameOrSql,
            IDictionary<string, object> args = null, bool rawSql = false)
            where T : class, new()
        {
            var query = BuildSql(session, spCallBuilder, spNameOrSql, args, rawSql);
            return query.AddEntity(typeof(T)).List<T>();
        }

        internal static void ExecuteNonQuery(ISession session, INHSpCallBuilder spCallBuilder, string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            var query = BuildSql(session, spCallBuilder, spNameOrSql, args, rawSql);
            query.ExecuteUpdate();
        }

        private static ISQLQuery BuildSql(ISession session, INHSpCallBuilder spCallBuilder, string spNameOrSql,
            IDictionary<string, object> args = null, bool rawSql = false)
        {
            var cmd = CommandBuilder.BuildCommand(spNameOrSql, args, rawSql);

            var command = cmd.IsRawSql
                ? cmd.CommandText
                : spCallBuilder.BuildSpCall(cmd);

            var query = session.CreateSQLQuery(command);

            var parameters = cmd.Parameters;

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    query.SetParameter(param.Name, param.Value);
                }
            }
            return query;
        }
    }
}