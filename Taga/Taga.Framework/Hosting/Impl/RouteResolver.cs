using System.Linq;
using Taga.Framework.Exceptions;
using Taga.Framework.Hosting.Configuration;

namespace Taga.Framework.Hosting.Impl
{
    public class RouteResolver : IRouteResolver
    {
        public RouteContext Resolve(IHttpRequest httpRequest)
        {
            var segments = httpRequest.Uri.Segments;
            
            var indexOfApi = -1;
            for (var i = 0; i < segments.Length; i++)
            {
                if (segments[i] == "api/")
                {
                    indexOfApi = i;
                    break;
                }
            }

            if (indexOfApi < 0)
            {
                throw Errors.F_RouteResolvingError;
            }

            // api/service/method
            // api/service/method/defaultParam

            if (segments.Length < indexOfApi + 3 || segments.Length > indexOfApi + 4)
            {
                throw Errors.F_RouteResolvingError;
            }

            var serviceRoute = segments[indexOfApi + 1].Trim('/');
            var methodRoute = segments[indexOfApi + 2].Trim('/');
            string defaultParam = null;

            if (segments.Length == indexOfApi + 4)
            {
                defaultParam = segments[indexOfApi + 3].Trim('/');   
            }
            
            var service = ServiceConfig.Current.ServiceMappings.SingleOrDefault(s => s.ServiceRoute == serviceRoute);

            if (service == null)
            {
                throw Errors.F_RouteResolvingError;
            }

            var method = service.MethodMappings.SingleOrDefault(m => m.MethodRoute == methodRoute && m.HttpMethod == httpRequest.Method);

            if (method == null)
            {
                throw Errors.F_RouteResolvingError;
            }

            return new RouteContext
            {
                Service = service,
                Method = method,
                HttpRequest = httpRequest,
                DefaultParameter = defaultParam
            };
        }
    }
}