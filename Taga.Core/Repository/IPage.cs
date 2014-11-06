
namespace Taga.Core.Repository
{
    public interface IPage<out T>
    {
        long CurrentPage { get; }

        long PageSize { get; }

        long TotalPages { get; }

        long TotalCount { get; }

        T[] Items { get; }
    }
}