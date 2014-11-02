using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Taga.Core.IoC;
using Taga.Core.Repository.Base;
using Taga.Core.Repository.Mapping;

namespace Taga.Core.Repository
{
    public interface IRepository
    {
        void Insert<T>(T entity) where T : class;
        void Update<T>(T entity) where T : class;
        void Delete<T>(T entity) where T : class;
        IQueryable<T> Select<T>() where T : class;

        IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
            where T : class;

        void NonQuery(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false);
    }

    internal class Page<T> : IPage<T>
    {
        public long CurrentPage { get; set; }

        public long PageSize { get; set; }

        public long TotalPages { get; set; }

        public long TotalCount { get; set; }

        public List<T> Items { get; set; }
    }

    public static class RepositoryExtensions
    {
        public static void Flush(this IRepository repo)
        {
            UnitOfWork.Current.Save();
        }

        public static void Save<T>(this IRepository repo, T entity) where T : class
        {
            var mapingProv = ServiceProvider.Provider.GetOrCreate<IMappingProvider>();

            var tableMapping = mapingProv.GetTableMapping<T>();

            if (!tableMapping.HasSingleAutoIncrementId)
            {
                throw new SaveException();
            }

            var isNew = tableMapping.IdColumns[0].PropertyInfo.GetValue(entity).Equals(0L);

            if (isNew)
            {
                repo.Insert(entity);
            }
            else
            {
                repo.Update(entity);
            }
        }

        public static void Delete<T>(this IRepository repo, Expression<Func<T, object>> propExpression,
            params object[] values) where T : class
        {
            PropertyInfo propInf;

            var body = propExpression.Body;
            if (body is UnaryExpression)
            {
                propInf = (PropertyInfo) ((MemberExpression) ((UnaryExpression) body).Operand).Member;
            }
            else
            {
                propInf = (PropertyInfo)((MemberExpression)body).Member;
            }

            var mappingProv = ServiceProvider.Provider.GetOrCreate<IMappingProvider>();

            var tableMapping = mappingProv.GetTableMapping<T>();

            var columnMapping = tableMapping.Columns.First(cm => cm.PropertyInfo == propInf);

            var paramNames = Enumerable.Range(0, values.Length).Select(i => String.Format("~p_{0}", i)).ToArray();

            var sql = new StringBuilder("DELETE FROM ")
                .Append(tableMapping.TableName)
                .Append(" WHERE ")
                .Append(columnMapping.ColumnName)
                .Append(" IN (")
                .Append(String.Join(",", paramNames))
                .Append(")")
                .ToString();

            var args = Enumerable.Range(0, values.Length).ToDictionary(i => paramNames[i], i => values[i]);

            repo.NonQuery(sql, args, true);
        }

        public static IList<T> QueryWithSp<T>(this IRepository repo, string spName, IDictionary<string, object> args = null)
            where T : class
        {
            return repo.Query<T>(spName, args);
        }

        public static IList<T> QueryWithSql<T>(this IRepository repo, string sql, IDictionary<string, object> args = null)
            where T : class
        {
            return repo.Query<T>(sql, args, true);
        }

        public static void ExecSp(this IRepository repo, string sql, IDictionary<string, object> args = null)
        {
            repo.NonQuery(sql, args);
        }

        public static void ExecSql(this IRepository repo, string sql, IDictionary<string, object> args = null)
        {
            repo.NonQuery(sql, args, true);
        }

        public static IPage<T> Page<T>(this IQueryable<T> query, int pageIndex, int pageSize) where T : class
        {
            var totalCount = query.Count();

            var pageCount = (totalCount - 1) / pageSize + 1;

            var items = query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new Page<T>
            {
                PageSize = pageSize,
                CurrentPage = pageIndex,
                TotalCount = totalCount,
                TotalPages = pageCount,
                Items = items.ToList()
            };
        }
    }
}