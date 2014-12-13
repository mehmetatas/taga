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

        private readonly INHSpCallBuilder _spCallBuilder;

        public NHRepository(INHSpCallBuilder spCallBuilder)
        {
            _spCallBuilder = spCallBuilder;
        }

        private IStatelessSession Session
        {
            get { return ((INHUnitOfWork) UnitOfWork).Session; }
        }

        public override void Insert<T>(T entity)
        {
            Session.Insert(entity);
        }

        public override void Update<T>(T entity)
        {
            Session.Update(entity);
        }

        public override void Delete<T>(T entity)
        {
            Session.Delete(entity);
        }

        public override IQueryable<T> Select<T>()
        {
            return Session.Query<T>();
        }

        public override IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            return ExecuteQuery<T>(Session, _spCallBuilder, spNameOrSql, args, rawSql);
        }

        public override void NonQuery(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            ExecuteNonQuery(Session, _spCallBuilder, spNameOrSql, args, rawSql);
        }

        internal static IList<T> ExecuteQuery<T>(IStatelessSession session, INHSpCallBuilder spCallBuilder, string spNameOrSql,
            IDictionary<string, object> args = null, bool rawSql = false)
            where T : class, new()
        {
            var query = BuildSql(session, spCallBuilder, spNameOrSql, args, rawSql);
            return query.AddEntity(typeof(T)).List<T>();
        }

        internal static void ExecuteNonQuery(IStatelessSession session, INHSpCallBuilder spCallBuilder, string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            var query = BuildSql(session, spCallBuilder, spNameOrSql, args, rawSql);
            query.ExecuteUpdate();
        }

        private static ISQLQuery BuildSql(IStatelessSession session, INHSpCallBuilder spCallBuilder, string spNameOrSql,
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