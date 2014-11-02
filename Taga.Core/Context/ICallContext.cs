
namespace Taga.Core.Context
{
    public interface ICallContext
    {
        object this[string key] { get; set; }
    }

    public static class CallContextExtensions
    {
        public static T Get<T>(this ICallContext ctx, string key)
        {
            var res = ctx[key];
            if (res is T)
            {
                return (T) res;
            }
            return default(T);
        }
    }
}
