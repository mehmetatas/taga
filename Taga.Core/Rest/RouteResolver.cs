using System.Net.Http;

namespace Taga.Core.Rest
{
    public class RouteResolver
    {
        private readonly HttpRequestMessage _request;

        public RouteResolver(HttpRequestMessage request)
        {
            _request = request;
        }

        public string ServiceRoute
        {
            get { return _request.RequestUri.Segments[2].Trim('/'); }
        }

        public string MethodRoute
        {
            get { return _request.RequestUri.Segments[3].Trim('/'); }
        }

        public string DefaultParam
        {
            get { return _request.RequestUri.Segments.Length == 5 ? _request.RequestUri.Segments[4].Trim('/') : null; }
        }
    }
}