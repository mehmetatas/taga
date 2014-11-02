
namespace Taga.Core.DynamicProxy
{
    public interface IProxyGenerator
    {
        T GetInstance<T>(ICallHandler callHandler) where T : class;
    }
}
