using System.Linq;
using System.Reflection;
using Taga.Core.Repository.Mapping;
using Taga.SimpLinq.QueryBuilder;

namespace Taga.Core.Repository.SimpLinq
{
    public class TagaPropertyFilter : IPropertyFilter
    {
        public bool Ignore(PropertyInfo propInf)
        {
            return !DatabaseMapping.AllowedTypes.Contains(propInf.PropertyType) &&
                   !propInf.PropertyType.IsEnum;
        }
    }
}
