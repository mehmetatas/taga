using System.Collections.Generic;

namespace Taga.Core.Repository
{
    public interface IPage<T>
    {
        long CurrentPage { get; }

        long PageSize { get; }

        long TotalPages { get; }

        long TotalCount { get; }

        List<T> Items { get; }
    }
}