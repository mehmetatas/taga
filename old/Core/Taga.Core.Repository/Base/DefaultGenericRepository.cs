using System;
using System.Linq;
using System.Linq.Expressions;
using Taga.Core.Model;

namespace Taga.Core.Repository.Base
{
    public abstract class DefaultGenericRepository<T> : GenericRepository<T> where T : class, IEntity
    {
        protected DefaultGenericRepository(IRepository<T> repository)
            : base(repository)
        {
        }

        public new virtual void Save(T entity)
        {
            base.Save(entity);
        }

        public new virtual void Delete(long id)
        {
            base.Delete(id);
        }

        public new virtual IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            return base.GetAll(filter);
        }

        public new virtual T Get(long id)
        {
            return base.Get(id);
        }
    }
}
