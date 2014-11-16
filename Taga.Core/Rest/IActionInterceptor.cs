using System;
using System.Reflection;

namespace Taga.Core.Rest
{
    public interface IActionInterceptor
    {
        void BeforeCall(MethodInfo actionMethod, object[] parameters);

        void AfterCall(MethodInfo actionMethod, object[] parameters, object returnValue);

        object OnException(MethodInfo actionMethod, object[] parameters, Exception exception);
    }
}
