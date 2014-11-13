using System.Reflection;

namespace Taga.Core.Repository.Mapping
{
    public interface IPropertyFilter
    {
        bool Ignore(PropertyInfo propertyInfo);
    }
}
