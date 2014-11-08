using System.Reflection;

namespace Taga.SimpLinq.QueryBuilder.Impl
{
    class DefaultPropertyFilter : IPropertyFilter
    {
        public bool Ignore(PropertyInfo propInf)
        {
            return false;
        }
    }
}
