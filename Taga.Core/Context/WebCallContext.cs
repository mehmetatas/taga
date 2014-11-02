using System.Web;

namespace Taga.Core.Context
{
    class WebCallContext : ICallContext
    {
        internal static readonly ICallContext Instance = new WebCallContext();

        private WebCallContext()
        {
        }

        public object this[string key]
        {
            get
            {
                return HttpContext.Current.Items.Contains(key)
                    ? HttpContext.Current.Items[key]
                    : null;
            }
            set
            {
                if (HttpContext.Current.Items.Contains(key))
                    HttpContext.Current.Items[key] = value;
                else
                    HttpContext.Current.Items.Add(key, value);
            }
        }
    }
}
