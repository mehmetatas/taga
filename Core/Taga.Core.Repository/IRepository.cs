using System;
using System.Linq;
using System.Linq.Expressions;
using Taga.Core.Model;

namespace Taga.Core.Repository
{
    public interface IRepository<T> where T : class, IEntity
    {
        void Save(T entity);
        void Delete(long id);
        IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null);
        T Get(long id);
    }
}
