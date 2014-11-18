using System.Reflection;
using Taga.Core.Repository.Mapping;

namespace Taga.UserApp.Core.Database
{
    public class UserAppPropertyFilter : IPropertyFilter
    {
        public bool Ignore(PropertyInfo propertyInfo)
        {
            var propType = propertyInfo.PropertyType;
            return propType.IsClass && propType != typeof(string) && propType != typeof(byte[]);
        }
    }
}