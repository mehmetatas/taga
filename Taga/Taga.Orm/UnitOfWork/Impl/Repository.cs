using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Taga.Orm.Db;

namespace Taga.Orm.UnitOfWork.Impl
{
    public class Repository : IRepository
    {
        private readonly IUnitOfWorkStack _stack;

        private UnitOfWork UnitOfWork => (UnitOfWork)_stack.Current;

        private IDb Db => UnitOfWork.Db;

        private void EnsureTransaction()
        {
            UnitOfWork.EnsureTransaction();
        }

        public Repository(IUnitOfWorkStack stack)
        {
            _stack = stack;
        }

        public void Insert<T>(T entity) where T : class, new()
        {
            EnsureTransaction();
            Db.Insert(entity);
        }

        public void Update<T>(T entity) where T : class, new()
        {
            EnsureTransaction();
            Db.Update(entity);
        }

        public void Delete<T>(T entity) where T : class, new()
        {
            EnsureTransaction();
            Db.Delete(entity);
        }

        public void DeleteMany<T>(Expression<Func<T, bool>> filter) where T : class, new()
        {
            EnsureTransaction();
            Db.DeleteMany(filter);
        }

        public T GetById<T>(object id) where T : class, new()
        {
            return Db.GetById<T>(id);
        }

        public IQuery<T> Select<T>() where T : class, new()
        {
            return Db.Select<T>();
        }

        public void Load<T, TProp>(IList<T> parentEntities, Expression<Func<T, TProp>> childPropExp, Expression<Func<TProp, object>> includeChildProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            Db.Load(parentEntities, childPropExp, includeChildProps);
        }

        public void LoadMany<T, TProp>(IList<T> parentEntities, Expression<Func<T, IList<TProp>>> childrenListExp, Expression<Func<TProp, object>> includeChildProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            Db.LoadMany(parentEntities, childrenListExp, includeChildProps);
        }
    }
}