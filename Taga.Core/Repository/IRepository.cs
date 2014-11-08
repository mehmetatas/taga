using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Taga.Core.IoC;
using Taga.Core.Repository.Base;
using Taga.Core.Repository.Mapping;
using Taga.Core.Repository.SimpLinq;

namespace Taga.Core.Repository
{
    public interface IRepository :
        IWriteRepository,
        IReadonlyRepository,
        IReadonlySqlRespository,
        IWriteSqlRespository
    {
    }

    public interface ISimpLinqRepository
    {
        ISimpLinqQuery<T> Query<T>() where T : class, new();
    }

    public interface IWriteRepository
    {
        void Insert<T>(T entity) where T : class, new();
        void Update<T>(T entity) where T : class, new();
        void Delete<T>(T entity) where T : class, new();
    }

    public interface IReadonlyRepository
    {
        IQueryable<T> Select<T>() where T : class, new();
    }

    public interface IReadonlySqlRespository
    {
        IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
            where T : class, new();
    }

    public interface IWriteSqlRespository
    {
        void NonQuery(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false);
    }

    public static class RepositoryExtensions
    {
        public static void Flush(this IRepository repo)
        {
            UnitOfWork.Current.Save();
        }

        public static void Save<T>(this IWriteRepository repo, T entity) where T : class, new()
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

        public static void Delete<T>(this IWriteSqlRespository repo, Expression<Func<T, object>> propExpression,
            params object[] values) where T : class, new()
        {
            MemberExpression memberExp;

            var body = propExpression.Body;
            if (body is UnaryExpression)
            {
                memberExp = (MemberExpression) ((UnaryExpression) body).Operand;
            }
            else
            {
                memberExp = (MemberExpression) body;
            }

            var propInf = (PropertyInfo) memberExp.Member;

            var mappingProv = ServiceProvider.Provider.GetOrCreate<IMappingProvider>();

            var tableMapping = mappingProv.GetTableMapping<T>();

            var columnMapping = tableMapping.Columns.First(cm => cm.PropertyInfo == propInf);

            var paramNames = Enumerable.Range(0, values.Length).Select(i => String.Format("p_{0}", i)).ToArray();

            var sql = new StringBuilder("DELETE FROM ")
                .Append(tableMapping.TableName)
                .Append(" WHERE ")
                .Append(columnMapping.ColumnName)
                .Append(" IN (")
                .Append(String.Join(",", paramNames.Select(p => String.Format("~{0}", p))))
                .Append(")")
                .ToString();

            var args = Enumerable.Range(0, values.Length).ToDictionary(i => paramNames[i], i => values[i]);

            repo.NonQuery(sql, args, true);
        }

        public static IList<T> QueryWithSp<T>(this IReadonlySqlRespository repo, string spName,
            IDictionary<string, object> args = null)
            where T : class, new()
        {
            return repo.Query<T>(spName, args);
        }

        public static IList<T> QueryWithSql<T>(this IReadonlySqlRespository repo, string sql,
            IDictionary<string, object> args = null)
            where T : class, new()
        {
            return repo.Query<T>(sql, args, true);
        }

        public static void ExecSp(this IWriteSqlRespository repo, string sql, IDictionary<string, object> args = null)
        {
            repo.NonQuery(sql, args);
        }

        public static void ExecSql(this IWriteSqlRespository repo, string sql, IDictionary<string, object> args = null)
        {
            repo.NonQuery(sql, args, true);
        }

        public static IPage<T> Page<T>(this IQueryable<T> query, int pageIndex, int pageSize) where T : class
        {
            var totalCount = query.Count();

            var pageCount = (totalCount - 1)/pageSize + 1;

            var items = query.Skip((pageIndex - 1)*pageSize).Take(pageSize).ToArray();

            return new Page<T>
            {
                PageSize = pageSize,
                CurrentPage = pageIndex,
                TotalCount = totalCount,
                TotalPages = pageCount,
                Items = items
            };
        }
    }
}