using Castle.DynamicProxy;

namespace Taga.Framework.DynamicProxy.Castle
{
    class CastleProxyInterceptorAdapter : IInterceptor
    {
        private readonly IProxyInterceptor _interceptor;

        public CastleProxyInterceptorAdapter(IProxyInterceptor interceptor)
        {
            _interceptor = interceptor;
        }

        public void Intercept(IInvocation invocation)
        {
            _interceptor.Intercept(new CastleInvocationContextAdapter(invocation));
        }
    }
}
