using System;
using System.Linq;
using System.Reflection;

namespace Taga.Core.DynamicProxy
{
    public static class ReflectionExtensions
    {
        public static Type[] GetParameterTypes(this MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().Select(pi => pi.ParameterType).ToArray();
        }

        public static bool HasAttribute<TAttribute>(this MemberInfo mi) where TAttribute : Attribute
        {
            if (mi is MethodInfo)
            {
                PropertyInfo property = null;
                if (mi.Name.StartsWith("get_"))
                {
                    property = mi.DeclaringType.GetProperties().FirstOrDefault(prop => prop.GetGetMethod() == mi);
                }
                else if (mi.Name.StartsWith("set_"))
                {
                    property = mi.DeclaringType.GetProperties().FirstOrDefault(prop => prop.GetSetMethod() == mi);
                }

                if (property != null)
                {
                    mi = property;
                }
            }
            return mi.GetCustomAttributes(typeof(TAttribute), false).Any();
        }
    }
}