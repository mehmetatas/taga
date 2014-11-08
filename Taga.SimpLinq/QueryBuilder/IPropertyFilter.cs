using System.Reflection;

namespace Taga.SimpLinq.QueryBuilder
{
    public interface IPropertyFilter
    {
        bool Ignore(PropertyInfo propInf);
    }
}
