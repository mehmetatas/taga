using System.Reflection;

namespace Taga.SimpLinq.QueryBuilder
{
    public interface IJoin
    {
        PropertyInfo LeftProperty { get; }
        PropertyInfo RightProperty { get; }
    }
}