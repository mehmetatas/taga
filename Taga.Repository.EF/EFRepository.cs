using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Taga.Core.Repository;
using Taga.Core.Repository.Base;
using Taga.Core.Repository.Command;
using Taga.Core.Repository.Command.Builders;

namespace Taga.Repository.EF
{
    public class EFRepository : IRepository
    {
        private static readonly ICommandBuilder CommandBuilder = new EntityFrameworkCommandBuilder();

        private readonly DbContext _dbContext;

        public EFRepository()
        {
            var uow = (IEFUnitOfWork)UnitOfWork.Current;
            _dbContext = uow.DbContext;
        }

        private DbSet<T> Set<T>() where T : class, new()
        {
            return _dbContext.Set<T>();
        }

        public void Insert<T>(T entity) where T : class, new()
        {
            SetState(entity, EntityState.Added);
        }

        public void Update<T>(T entity) where T : class, new()
        {
            SetState(entity, EntityState.Modified);
        }

        public void Delete<T>(T entity) where T : class, new()
        {
            SetState(entity, EntityState.Deleted);
        }

        public IQueryable<T> Select<T>() where T : class, new()
        {
            return Set<T>();
        }

        public IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
            where T : class, new()
        {
            return ExecuteQuery<T>(_dbContext, spNameOrSql, args, rawSql);
        }

        public void NonQuery(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
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
