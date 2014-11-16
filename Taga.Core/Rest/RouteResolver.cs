using System;
using System.Net.Http;

namespace Taga.Core.Rest
{
    public class RouteResolver
    {
        public RouteResolver(HttpRequestMessage request)
        {
            var segments = request.RequestUri.Segments;

            if (segments.Length < 4 || segments.Length > 5)
            {
                throw new NotSupportedException("Number of request uri segments must be 4 or 5!");
            }

            ServiceRoute = segments[2].Trim('/');
            MethodRoute = segments[3].Trim('/');

            if (segments.Length == 5)
            {
                DefaultParam = segments[4].Trim('/');   
            }

            switch (request.Method.Method)
            {
                case "POST":
                    HttpMethodType = HttpMethodType.Post;
                    break;
                case "GET":
                    HttpMethodType = HttpMethodType.Get;
                    break;
                case "PUT":
                    HttpMethodType = HttpMethodType.Put;
                    break;
                case "DELETE":
                    HttpMethodType = HttpMethodType.Delete;
                    break;
                default:
                    throw new NotSupportedException("Unsupported http method: " + request.Method.Method);
            }
        }

        public string ServiceRoute { get; private set; }
        public string MethodRoute { get; private set; }
        public string DefaultParam { get; private set; }
        public HttpMethodType HttpMethodType { get; private set; }
    }
}