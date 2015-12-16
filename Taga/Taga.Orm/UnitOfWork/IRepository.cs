using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Taga.Orm.Db;

namespace Taga.Orm.UnitOfWork
{
    public interface IRepository
    {
        void Insert<T>(T entity) where T : class, new();

        void Update<T>(T entity) where T : class, new();

        void Delete<T>(T entity) where T : class, new();

        void DeleteMany<T>(Expression<Func<T, bool>> filter) where T : class, new();

        T GetById<T>(object id) where T : class, new();

        IQuery<T> Select<T>() where T : class, new();

        void Load<T, TProp>(IList<T> parentEntities, Expression<Func<T, TProp>> childPropExp, Expression<Func<TProp, object>> includeChildProps = null)
            where T : class, new()
            where TProp : class, new();

        void LoadMany<T, TProp>(IList<T> parentEntities, Expression<Func<T, IList<TProp>>> childrenListExp, Expression<Func<TProp, object>> includeChildProps = null)
            where T : class, new()
            where TProp : class, new();
    }

    public static class RepositoryExtensions
    {
        public static void Insert<T>(this IRepository db, params T[] entities) where T : class, new()
        {
            foreach (var entity in entities)
            {
                db.Insert(entity);
            }
        }

        public static void Insert<T>(this IRepository db, IEnumerable<T> entities) where T : class, new()
        {
            foreach (var entity in entities)
            {
                db.Insert(entity);
            }
        }

        public static void Update<T>(this IRepository db, params T[] entities) where T : class, new()
        {
            foreach (var entity in entities)
            {
                db.Update(entity);
            }
        }

        public static void Update<T>(this IRepository db, IEnumerable<T> entities) where T : class, new()
        {
            foreach (var entity in entities)
            {
                db.Update(entity);
            }
        }

        public static void Delete<T>(this IRepository db, params T[] entities) where T : class, new()
        {
            foreach (var entity in entities)
            {
                db.Delete(entity);
            }
        }

        public static void Delete<T>(this IRepository db, IEnumerable<T> entities) where T : class, new()
        {
            foreach (var entity in entities)
            {
                db.Delete(entity);
            }
        }

        public static void Load<T, TProp>(this IRepository db, T parentEntity, Expression<Func<T, TProp>> childPropExp, Expression<Func<TProp, object>> includeChildProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            db.Load(new[] { parentEntity }, childPropExp, includeChildProps);
        }

        public static void LoadMany<T, TProp>(this IRepository db, T parentEntity, Expression<Func<T, IList<TProp>>> childrenListExp, Expression<Func<TProp, object>> includeChildProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            db.LoadMany(new[] { parentEntity }, childrenListExp, includeChildProps);
        }
    }
}