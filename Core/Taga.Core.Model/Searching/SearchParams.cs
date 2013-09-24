using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Taga.Core.Model.Searching
{
    public class SearchParams<T> where T : class, IEntity
    {
        public SearchParams()
        {
            Sortings = new List<Sorting<T>>();
        }

        private PagingInfo _pagingInfo;
        public PagingInfo PagingInfo
        {
            get { return _pagingInfo ?? (_pagingInfo = new PagingInfo()); }
            set { _pagingInfo = value; }
        }

        public Expression<Func<T, bool>> Filter { get; set; }
        public List<Sorting<T>> Sortings { get; set; }

        public virtual SearchResult<T> ApplyTo(IQueryable<T> entities)
        {
            entities = ApplyFilter(entities);

            var totalCount = entities.Count();

            entities = ApplySorting(entities);
            entities = ApplyPaging(entities);

            return new SearchResult<T>
                       {
                           Results = entities,
                           TotalCount = totalCount,
                           PagingInfo = PagingInfo
                       };
        }

        protected virtual IQueryable<T> ApplyPaging(IQueryable<T> entities)
        {
            return _pagingInfo == null
                ? entities
                : entities.Skip(_pagingInfo.Start - 1).Take(_pagingInfo.PageSize);
        }

        protected virtual IQueryable<T> ApplyFilter(IQueryable<T> entities)
        {
            return Filter == null
                ? entities
                : entities.Where(Filter);
        }

        protected virtual IQueryable<T> ApplySorting(IQueryable<T> entities)
        {
            if (!Sortings.Any())
                return entities;

            var sortArr = Sortings.ToArray();

            var sort = sortArr[0];
            var res = sort.Descending
                    ? entities.OrderByDescending(sort.Expression)
                    : entities.OrderBy(sort.Expression);

            for (var i = 1; i < sortArr.Length; i++)
            {
                sort = sortArr[i];
                res = sort.Descending
                       ? res.ThenByDescending(sort.Expression)
                       : res.ThenBy(sort.Expression);
            }

            return res;
        }
    }
}
