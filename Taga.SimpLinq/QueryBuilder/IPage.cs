namespace Taga.SimpLinq.QueryBuilder
{
    public interface IPage
    {
        int PageIndex { get; }
        int PageSize { get; }
    }
}