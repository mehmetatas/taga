using System.Reflection;

namespace Taga.SimpLinq.QueryBuilder
{
    public interface IOrderBy
    {
        PropertyInfo OrderProperty { get; }
        bool Descending { get; }
    }
}