using System;
using Castle.DynamicProxy;

namespace Taga.Framework.DynamicProxy.Castle
{
    public class CastleDynamicProxyFactory : IDynamicProxyFactory
    {
        private readonly ProxyGenerator _proxyGenerator;

        public CastleDynamicProxyFactory()
        {
            _proxyGenerator = new ProxyGenerator();
        }

        public object Create(Type type, IProxyInterceptor interceptor, object target = null)
        {
            if (type.IsInterface)
            {
                return target == null
                    ? _proxyGenerator.CreateInterfaceProxyWithoutTarget(type, new CastleProxyInterceptorAdapter(interceptor))
                    : _proxyGenerator.CreateInterfaceProxyWithTarget(type, target, new CastleProxyInterceptorAdapter(interceptor));
            }

            return target == null
                ? _proxyGenerator.CreateClassProxy(type, new CastleProxyInterceptorAdapter(interceptor))
                : _proxyGenerator.CreateClassProxyWithTarget(type, target, new CastleProxyInterceptorAdapter(interceptor));
        }
    }
}
