using System;
using Taga.Framework.Hosting.Configuration;

namespace Taga.Framework.Hosting
{
    public class RouteContext
    {
        public IHttpRequest HttpRequest { get; set; }
        public ServiceMapping Service { get; set; }
        public MethodMapping Method { get; set; }
        public string DefaultParameter { get; set; }
        public object[] Parameters { get; set; }
        public object ReturnValue { get; set; }
        public Exception Exception{ get; set; }
    }
}