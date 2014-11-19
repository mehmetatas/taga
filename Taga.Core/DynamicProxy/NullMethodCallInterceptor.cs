using System;
using System.Reflection;

namespace Taga.Core.DynamicProxy
{
    public class NullMethodCallInterceptor : IMethodCallInterceptor
    {
        public static readonly IMethodCallInterceptor Instance = new NullMethodCallInterceptor();

        private NullMethodCallInterceptor()
        {
        }

        public object BeforeCall(object obj, MethodInfo mi, object[] args)
        {
            return null;
        }

        public void AfterCall(object obj, MethodInfo mi, object[] args, object returnValue)
        {
        }

        public void OnError(object obj, MethodInfo mi, object[] args, Exception exception)
        {
        }
    }
}