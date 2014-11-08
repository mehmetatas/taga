namespace Taga.Core.Repository
{
    public class Page<T> : IPage<T>
    {
        public long CurrentPage { get; set; }

        public long PageSize { get; set; }

        public long TotalPages { get; set; }

        public long TotalCount { get; set; }

        public T[] Items { get; set; }
    }
}