using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Taga.Core.Repository.Command;
using Taga.Core.Repository.Command.Builders;

namespace Taga.Repository.EF
{
    public class EFRepository : Core.Repository.Base.Repository
    {
        private static readonly ICommandBuilder CommandBuilder = new EntityFrameworkCommandBuilder();

        private readonly DbContext _dbContext;

        public EFRepository()
        {
            var uow = (IEFUnitOfWork)UnitOfWork;
            _dbContext = uow.DbContext;
        }

        private DbSet<T> Set<T>() where T : class, new()
        {
            return _dbContext.Set<T>();
        }

        public override void Insert<T>(T entity)
        {
            SetState(entity, EntityState.Added);
        }

        public override void Update<T>(T entity)
        {
            SetState(entity, EntityState.Modified);
        }

        public override void Delete<T>(T entity)
        {
            SetState(entity, EntityState.Deleted);
        }

        public override IQueryable<T> Select<T>()
        {
            return Set<T>();
        }

        public override IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            return ExecuteQuery<T>(_dbContext, spNameOrSql, args, rawSql);
        }

        public override void NonQuery(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            ExecuteNonQuery(_dbContext, spNameOrSql, args, rawSql);
        }

        internal static IList<T> ExecuteQuery<T>(DbContext dbContext, string spNameOrSql,
            IDictionary<string, object> args = null,
            bool rawSql = false) where T : class, new()
        {
            var cmd = CommandBuilder.BuildCommand(spNameOrSql, args, rawSql);
            return dbContext.Database.SqlQuery<T>(cmd.CommandText, cmd.Parameters.Select(p => p.Value).ToArray()).ToList();
        }

        internal static void ExecuteNonQuery(DbContext dbContext, string spNameOrSql,
            IDictionary<string, object> args = null,
            bool rawSql = false)
        {
            var cmd = CommandBuilder.BuildCommand(spNameOrSql, args, rawSql);
            dbContext.Database.ExecuteSqlCommand(cmd.CommandText, cmd.Parameters.Select(p => p.Value).ToArray());
        }

        private void SetState<T>(T entity, EntityState state) where T : class, new()
        {
            var entry = _dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                Set<T>().Attach(entity);
            }
            entry.State = state;
        }
    }
}
