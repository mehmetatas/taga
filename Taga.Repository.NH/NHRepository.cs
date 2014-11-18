using NHibernate;
using NHibernate.Linq;
using System.Collections.Generic;
using System.Linq;
using Taga.Core.Repository.Command;
using Taga.Core.Repository.Command.Builders;

namespace Taga.Repository.NH
{
    public class NHRepository : Core.Repository.Base.Repository
    {
        private static readonly ICommandBuilder CommandBuilder = new NHibernateCommandBuilder();

        private readonly ISession _session;
        private readonly INHSpCallBuilder _spCallBuilder;

        public NHRepository(INHSpCallBuilder spCallBuilder)
        {
            var uow = (INHUnitOfWork)UnitOfWork;
            _session = uow.Session;
            _spCallBuilder = spCallBuilder;
        }

        public override void Insert<T>(T entity)
        {
            _session.Save(entity);
        }

        public override void Update<T>(T entity)
        {
            _session.Update(entity);
        }

        public override void Delete<T>(T entity)
        {
            _session.Delete(entity);
        }

        public override IQueryable<T> Select<T>()
        {
            return _session.Query<T>();
        }

        public override IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            return ExecuteQuery<T>(_session, _spCallBuilder, spNameOrSql, args, rawSql);
        }

        public override void NonQuery(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
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