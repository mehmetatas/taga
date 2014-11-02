using System.Collections.Generic;
using System.Data;
using System.Linq;
using Taga.Core.Repository;
using Taga.Core.Repository.Base;

namespace Taga.Repository.Hybrid
{
    public class HybridRepository : IRepository
    {
        private readonly IHybridUnitOfWork _uow;
        
        public HybridRepository()
        {
            _uow = (IHybridUnitOfWork) UnitOfWork.Current;
        }

        public void Insert<T>(T entity) where T : class
        {
            _uow.Insert(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            _uow.Update(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            _uow.Delete(entity);
        }

        public IQueryable<T> Select<T>() where T : class
        {
            return _uow.QueryProvider.Select<T>();
        }

        public IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false) where T : class
        {
            return _uow.QueryProvider.Query<T>(spNameOrSql, args, rawSql);
        }

        public void NonQuery(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            using (var cmd = _uow.CreateCommand())
            {
                cmd.CommandType = rawSql
                    ? CommandType.Text
                    : CommandType.StoredProcedure;

                cmd.CommandText = spNameOrSql;

                if (args != null)
                {
                    foreach (var arg in args)
                    {
                        var param = cmd.CreateParameter();

                        param.ParameterName = arg.Key;
                        param.Value = arg.Value;

                        cmd.Parameters.Add(param);
                    }
                }

                cmd.ExecuteNonQuery();
            }
        }
    }
}
