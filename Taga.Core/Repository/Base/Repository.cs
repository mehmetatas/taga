using System.Collections.Generic;
using System.Linq;

namespace Taga.Core.Repository.Base
{
    public abstract class Repository : IRepository
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected Repository()
        {
            UnitOfWork = Base.UnitOfWork.Current;
        }

        public abstract void Insert<T>(T entity) where T : class, new();
        public abstract void Update<T>(T entity) where T : class, new();
        public abstract void Delete<T>(T entity) where T : class, new();
        public abstract IQueryable<T> Select<T>() where T : class, new();
        public abstract IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false) where T : class, new();
        public abstract void NonQuery(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false);

        public void Flush()
        {
            UnitOfWork.Save();
        }
    }
}
