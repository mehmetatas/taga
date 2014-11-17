using System;
using System.Reflection;

namespace Taga.Core.Rest
{
    public interface IActionInterceptor
    {
        void BeforeCall(IRequestContext ctx, MethodInfo actionMethod, object[] parameters);

        void AfterCall(IRequestContext ctx, MethodInfo actionMethod, object[] parameters, object returnValue);

        object OnException(IRequestContext ctx, MethodInfo actionMethod, object[] parameters, Exception exception);
    }
}
