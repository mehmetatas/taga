using Taga.Core.DynamicProxy;

namespace Taga.Impl.DynamicProxy.Builder
{
    public class ProxyBuilder : IProxyGenerator
    {
        public T GetInstance<T>(ICallHandler callHandler) where T : class
        {
            return Proxy.Of<T>(callHandler);
        }
    }
}
