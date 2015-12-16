using System.Collections.Generic;
using System.Linq;

namespace Taga.Orm.Sql
{
    public class Page<T>
    {
        public Page(int page, int pageSize, int totalCount, IEnumerable<T> items)
        {
            PageIndex = page;
            PageSize = pageSize;
            TotalCount = totalCount;
            PageCount = (totalCount - 1) / pageSize + 1;
            Items = items.ToArray();
        }

        public bool HasMore => PageIndex < PageCount;

        public int PageIndex { get; }
        public int PageSize { get; private set; }
        public int PageCount { get; }
        public int TotalCount { get; private set; }

        public T[] Items { get; private set; }
    }
}