using System;

namespace Taga.Framework.DynamicProxy
{
    public interface IDynamicProxyFactory
    {
        object Create(Type type, IProxyInterceptor interceptor, object target = null);
    }
}
