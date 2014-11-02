using System.Web;
using RuntimeContext = System.Runtime.Remoting.Messaging.CallContext;

namespace Taga.Core.Context
{
    public static class CallContext
    {
        public static ICallContext Current
        {
            get
            {
                return HttpContext.Current == null
                    ? RuntimeContext.Instance
                    : WebCallContext.Instance;
            }
        }
    }
}