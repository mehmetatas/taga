using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Taga.Core.DynamicProxy
{
    internal static class ReflectionExtensions
    {
        public static bool HasDefaultConstructor(this Type type)
        {
            return
                type.GetConstructors(BindingFlags.Public | BindingFlags.Instance).Any(
                    ctor => !ctor.GetParameters().Any());
        }

        public static Type[] GetParameterTypes(this MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().Select(pi => pi.ParameterType).ToArray();
        }

        public static MethodBuilder GetMethodBuilder(this TypeBuilder typeBuilder, MethodInfo mi)
        {
            // MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual
            return typeBuilder.DefineMethod(mi.Name, mi.Attributes, mi.ReturnType, mi.GetParameterTypes());
        }

        public static MethodInfo[] GetVirtualMethods(this Type type)
        {
            return type.GetMethods().Where(mi => mi.IsVirtual).ToArray();
        }

        public static bool HasAttribute<TAttribute>(this MemberInfo mi) where TAttribute : Attribute
        {
            if (mi is MethodInfo)
            {
                PropertyInfo property = null;
                if (mi.Name.StartsWith("get_"))
                    property = mi.DeclaringType.GetProperties().FirstOrDefault(prop => prop.GetGetMethod() == mi);
                else if (mi.Name.StartsWith("set_"))
                    property = mi.DeclaringType.GetProperties().FirstOrDefault(prop => prop.GetSetMethod() == mi);

                if (property != null)
                    mi = property;
            }
            return mi.GetCustomAttributes(typeof (TAttribute), false).Any();
        }
    }
}