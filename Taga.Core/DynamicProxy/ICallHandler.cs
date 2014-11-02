using System;
using System.Reflection;

namespace Taga.Core.DynamicProxy
{
    public interface ICallHandler
    {
        object BeforeCall(object obj, MethodInfo mi, object[] args);
        void AfterCall(object obj, MethodInfo mi, object[] args, object returnValue);
        void OnError(object obj, MethodInfo mi, object[] args, Exception exception);
    }
}