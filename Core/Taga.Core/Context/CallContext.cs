using System.Web;

using NetCallContext = System.Runtime.Remoting.Messaging.CallContext;

namespace Taga.Core.Context
{
    public static class CallContext
    {
        public static T Get<T>(string key)
        {
            var res = Get(key);
            if (res is T)
                return (T)res;
            return default(T);
        }

        public static object Get(string key)
        {
            if (HttpContext.Current == null)
                return NetCallContext.GetData(key);

            return HttpContext.Current.Items.Contains(key)
                       ? HttpContext.Current.Items[key]
                       : null;
        }

        public static void Set(string key, object value)
        {
            if (HttpContext.Current == null)
                NetCallContext.SetData(key, value);
            else if (HttpContext.Current.Items.Contains(key))
                HttpContext.Current.Items[key] = value;
            else
                HttpContext.Current.Items.Add(key, value);
        }
    }
}
