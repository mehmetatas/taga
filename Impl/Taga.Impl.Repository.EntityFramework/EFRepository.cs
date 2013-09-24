using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Taga.Core.Model;
using Taga.Core.Repository.Base;

namespace Taga.Impl.Repository.EntityFramework
{
    public class EFRepository<T> : Repository<T> where T : class, IEntity, new()
    {
        private DbSet<T> _dbSet;
        private DbContext _dbContext;

        public EFRepository()
        {
            SetContext();
        } 

        #region IRepository<T>

        public override void Save(T entity)
        {
            var entry = _dbContext.Entry(entity);

            if (entry.State != EntityState.Detached)
                return; // context already knows about entity, don't do anything

            if (entity.Id < 1)
            {
                _dbSet.Add(entity);
                return;
            }

            var attachedEntity = _dbSet.Local.SingleOrDefault(e => e.Id == entity.Id);
            if (attachedEntity != null)
                _dbContext.Entry(attachedEntity).State = EntityState.Detached;
            entry.State = EntityState.Modified;
        }

        public override void Delete(long id)
        {
            var entity = _dbSet.Local.SingleOrDefault(e => e.Id == id);
            if (entity == null)
            {
                entity = new T { Id = id };
                _dbSet.Attach(entity);
            }
            _dbSet.Remove(entity);
        }

        public override IQueryable<T> GetAll(Expression<Func<T, bool>> filter = null)
        {
            return filter == null
                       ? _dbSet
                       : _dbSet.Where(filter);
        }

        public override T Get(long id)
        {
            return _dbSet.SingleOrDefault(e => e.Id == id);
        }

        #endregion
        
        private void SetContext()
        {
            if (!(UnitOfWork is EFUnitOfWork))
                throw new InvalidOperationException("Unit Of Work must be of type " + typeof(EFUnitOfWork));

            _dbContext = (UnitOfWork as EFUnitOfWork).DbContext;
            _dbSet = _dbContext.Set<T>();
        }
    }
}
