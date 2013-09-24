using System;
using System.Linq;
using System.Linq.Expressions;
using Taga.Core.Model;

namespace Taga.Core.Repository.Base
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class, IEntity
    {
        protected GenericRepository(IRepository<T> repository)
        {
            Repository = repository;
        }

        IRepository<T> IGenericRepository<T>.Repository { get; set; }

        private IRepository<T> Repository
        {
            get { return (this as IGenericRepository<T>).Repository; }
            set { (this as IGenericRepository<T>).Repository = value; }
        }

        protected virtual void Save(T entity)
        {
            Repository.Save(entity);
        }

        protected virtual void Delete(long id)
        {
            Repository.Delete(id);
        }

        protected virtual IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            return Repository.GetAll(filter);
        }

        protected virtual T Get(long id)
        {
            return Repository.Get(id);
        }
    }
}
