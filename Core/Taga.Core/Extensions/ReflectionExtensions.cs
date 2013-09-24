using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Taga.Core.Extensions
{
    public static class ReflectionExtensions
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

        public static object GetDefaultValue(this Type t)
        {
            return typeof (ReflectionExtensions).GetMethod("Default").MakeGenericMethod(t).Invoke(null, null);
        }

        public static T Default<T>()
        {
            return default(T);
        }
    }
}
