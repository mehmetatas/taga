using System;
using System.Linq;
using System.Linq.Expressions;
using Taga.Core.Model;

namespace Taga.Core.Repository.Base
{
    public abstract class Repository<T> : IRepository<T> where T : class, IEntity
    {
        protected readonly IUnitOfWork UnitOfWork;

        protected Repository()
        {
            UnitOfWork = Base.UnitOfWork.Current;
        } 

        public abstract void Save(T entity);
        public abstract void Delete(long id);
        public abstract IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null);
        public abstract T Get(long id);
    }
}
