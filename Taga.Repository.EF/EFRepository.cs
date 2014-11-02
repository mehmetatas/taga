using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using Taga.Core.Repository;
using Taga.Core.Repository.Base;

namespace Taga.Repository.EF
{
    public class EFRepository : IRepository
    {
        private readonly DbContext _dbContext;

        public EFRepository()
        {
            var uow = (IEFUnitOfWork)UnitOfWork.Current;
            _dbContext = uow.DbContext;
        }

        private DbSet<T> Set<T>() where T : class
        {
            return _dbContext.Set<T>();
        }

        public void Insert<T>(T entity) where T : class
        {
            SetState(entity, EntityState.Added);
        }

        public void Update<T>(T entity) where T : class
        {
            SetState(entity, EntityState.Modified);
        }

        public void Delete<T>(T entity) where T : class
        {
            SetState(entity, EntityState.Deleted);
        }

        public IQueryable<T> Select<T>() where T : class
        {
            return Set<T>();
        }

        public IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false) where T : class
        {
            return ExecuteQuery<T>(_dbContext, spNameOrSql, args, rawSql);
        }

        public void NonQuery(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            ExecuteNonQuery(_dbContext, spNameOrSql, args, rawSql);
        }

        internal static IList<T> ExecuteQuery<T>(DbContext dbContext, string spNameOrSql, IDictionary<string, object> args = null,
            bool rawSql = false) where T : class
        {
            //TODO: parametreli SP????
            return dbContext.Database.SqlQuery<T>(spNameOrSql, args == null ? null : args.Values).ToList();
        }

        private static void ExecuteNonQuery(DbContext dbContext, string spNameOrSql, IDictionary<string, object> args = null,
            bool rawSql = false)
        {
            var connection = dbContext.Database.Connection;

            var connectionClosed = connection.State == ConnectionState.Closed;

            try
            {
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandType = rawSql
                        ? CommandType.Text
                        : CommandType.StoredProcedure;

                    //TODO: parametreli SP????
                    cmd.CommandText = spNameOrSql;

                    if (args != null)
                    {
                        cmd.Parameters.AddRange(args.Select(arg =>
                        {
                            var param = cmd.CreateParameter();
                            param.ParameterName = arg.Key;
                            param.Value = arg.Value;
                            return param;
                        }).ToArray());
                    }

                    if (connectionClosed)
                    {
                        connection.Open();
                    }

                    cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (connectionClosed)
                {
                    connection.Close();
                }   
            }
        }

        private void SetState<T>(T entity, EntityState state) where T : class
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
