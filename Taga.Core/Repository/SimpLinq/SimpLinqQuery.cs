using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Taga.SimpLinq.QueryBuilder;

namespace Taga.Core.Repository.SimpLinq
{
    public class SimpLinqQuery<T> : ISimpLinqQuery<T> where T : class, new()
    {
        private readonly ISelectQueryBuilder _queryBuilder;
        private readonly ISimpLinqDataAdapter _adapter;
        private readonly ISimpLinqResolver _resolver;

        public SimpLinqQuery(ISimpLinqResolver resolver, ISimpLinqDataAdapter adapter,
            IPropertyFilter propertyFilter = null)
        {
            _resolver = resolver;
            _adapter = adapter;
            _queryBuilder = Select.From<T>(propertyFilter);
        }

        public ISimpLinqQuery<T> Include(params Expression<Func<T, object>>[] propExpressions)
        {
            _queryBuilder.Include(propExpressions);
            return this;
        }

        public ISimpLinqQuery<T> Exclude(params Expression<Func<T, object>>[] propExpressions)
        {
            _queryBuilder.Exclude(propExpressions);
            return this;
        }

        public ISimpLinqQuery<T> Where(Expression<Func<T, bool>> filter)
        {
            _queryBuilder.Where(filter);
            return this;
        }

        public ISimpLinqQuery<T> OrderBy(Expression<Func<T, object>> propExpression, bool desc = false)
        {
            _queryBuilder.OrderBy(propExpression, desc);
            return this;
        }

        public ISimpLinqQuery<T> LeftJoin<TJoined>(Expression<Func<T, object>> propLeft,
            Expression<Func<TJoined, object>> propRight) where TJoined : class, new()
        {
            _queryBuilder.LeftJoin(propLeft, propRight);
            return this;
        }

        public ISimpLinqQuery<T> InnerJoin<TJoined>(Expression<Func<T, object>> propLeft,
            Expression<Func<TJoined, object>> propRight) where TJoined : class, new()
        {
            _queryBuilder.InnerJoin(propLeft, propRight);
            return this;
        }

        public ISimpLinqQuery<T> RightJoin<TJoined>(Expression<Func<T, object>> propLeft,
            Expression<Func<TJoined, object>> propRight) where TJoined : class, new()
        {
            _queryBuilder.RightJoin(propLeft, propRight);
            return this;
        }

        public ISimpLinqQuery<T> Include<TJoined>(params Expression<Func<TJoined, object>>[] propExpressions)
            where TJoined : class, new()
        {
            _queryBuilder.Include(propExpressions);
            return this;
        }

        public ISimpLinqQuery<T> Exclude<TJoined>(params Expression<Func<TJoined, object>>[] propExpressions)
            where TJoined : class, new()
        {
            _queryBuilder.Exclude(propExpressions);
            return this;
        }

        public ISimpLinqQuery<T> Where<TJoined>(Expression<Func<TJoined, bool>> filter) where TJoined : class, new()
        {
            _queryBuilder.Where(filter);
            return this;
        }

        public ISimpLinqQuery<T> OrderBy<TJoined>(Expression<Func<TJoined, object>> propExpression, bool desc = false)
            where TJoined : class, new()
        {
            _queryBuilder.OrderBy(propExpression, desc);
            return this;
        }

        public T FirstOrDefault(Expression<Func<T, bool>> filter = null)
        {
            var query = _queryBuilder.FirstOrDefault(filter);
            var cmd = _resolver.Resolve(query);
            return _adapter.List<T>(cmd).FirstOrDefault();
        }

        public IPage<T> Page(int pageIndex, int pageSize)
        {
            var query = _queryBuilder.Page(pageIndex, pageSize);
            var cmd = _resolver.Resolve(query);
            return _adapter.Page<T>(cmd, pageIndex, pageSize);
        }

        public IList<T> List()
        {
            var query = _queryBuilder.List();
            var cmd = _resolver.Resolve(query);
            return _adapter.List<T>(cmd);
        }

        public ISimpLinqSelect<T, T1> SelectWith<T1>() where T1 : class, new()
        {
            return new SimpleLinqSelect<T, T1>(_adapter, _queryBuilder, _resolver);
        }

        public ISimpLinqSelect<T, T1, T2> SelectWith<T1, T2>()
            where T1 : class, new()
            where T2 : class, new()
        {
            return new SimpleLinqSelect<T, T1, T2>(_adapter, _queryBuilder, _resolver);
        }
    }

    internal class SimpleLinqSelect<T1, T2> : ISimpLinqSelect<T1, T2>
        where T1 : class, new()
        where T2 : class, new()
    {
        private readonly ISimpLinqDataAdapter _adapter;
        private readonly ISelectQueryBuilder _queryBuilder;
        private readonly ISimpLinqResolver _resolver;

        public SimpleLinqSelect(ISimpLinqDataAdapter adapter, ISelectQueryBuilder queryBuilder,
            ISimpLinqResolver resolver)
        {
            _adapter = adapter;
            _queryBuilder = queryBuilder;
            _resolver = resolver;
        }

        public Tuple<T1, T2> FirstOrDefault(Expression<Func<T1, bool>> filter1 = null,
            Expression<Func<T2, bool>> filter2 = null)
        {
            var query = _queryBuilder.Where(filter1).Where(filter2).Page(1, 1);
            var cmd = _resolver.Resolve(query);
            return _adapter.List<T1, T2>(cmd).FirstOrDefault();
        }

        public IPage<Tuple<T1, T2>> Page(int pageIndex, int pageSize)
        {
            var query = _queryBuilder.Page(pageIndex, pageSize);
            var cmd = _resolver.Resolve(query);
            return _adapter.Page<T1, T2>(cmd, pageIndex, pageSize);
        }

        public IList<Tuple<T1, T2>> List()
        {
            var query = _queryBuilder.List();
            var cmd = _resolver.Resolve(query);
            return _adapter.List<T1, T2>(cmd);
        }
    }

    internal class SimpleLinqSelect<T1, T2, T3> : ISimpLinqSelect<T1, T2, T3>
        where T1 : class, new()
        where T2 : class, new()
        where T3 : class, new()
    {
        private readonly ISimpLinqDataAdapter _adapter;
        private readonly ISelectQueryBuilder _queryBuilder;
        private readonly ISimpLinqResolver _resolver;

        public SimpleLinqSelect(ISimpLinqDataAdapter adapter, ISelectQueryBuilder queryBuilder,
            ISimpLinqResolver resolver)
        {
            _adapter = adapter;
            _queryBuilder = queryBuilder;
            _resolver = resolver;
        }

        public Tuple<T1, T2, T3> FirstOrDefault(Expression<Func<T1, bool>> filter1 = null,
            Expression<Func<T2, bool>> filter2 = null, Expression<Func<T3, bool>> filter3 = null)
        {
            var query = _queryBuilder.Where(filter1).Where(filter2).Where(filter3).Page(1, 1);
            var cmd = _resolver.Resolve(query);
            return _adapter.List<T1, T2, T3>(cmd).FirstOrDefault();
        }

        public IPage<Tuple<T1, T2, T3>> Page(int pageIndex, int pageSize)
        {
            var query = _queryBuilder.Page(pageIndex, pageSize);
            var cmd = _resolver.Resolve(query);
            return _adapter.Page<T1, T2, T3>(cmd, pageIndex, pageSize);
        }

        public IList<Tuple<T1, T2, T3>> List()
        {
            var query = _queryBuilder.List();
            var cmd = _resolver.Resolve(query);
            return _adapter.List<T1, T2, T3>(cmd);
        }
    }
}