using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Taga.Core.IoC;
using Taga.Core.Json;

namespace Taga.Core.Rest
{
    public class GenericApiHandler : DelegatingHandler
    {
        private readonly ServiceConfig _serviceConfig;

        public GenericApiHandler(HttpConfiguration httpConfig, ServiceConfig serviceConfig)
        {
            InnerHandler = new HttpControllerDispatcher(httpConfig);
            _serviceConfig = serviceConfig;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(responseToCompleteTask =>
            {
                var response = responseToCompleteTask.Result;

                var httpMethod = HttpMethodType.Post;
                if (request.Method.Method == "GET")
                    httpMethod = HttpMethodType.Get;
                else if (request.Method.Method == "PUT")
                    httpMethod = HttpMethodType.Put;
                else if (request.Method.Method == "DELETE")
                    httpMethod = HttpMethodType.Delete;

                var resolver = new RouteResolver(request);

                var service = _serviceConfig.ServiceMappings.SingleOrDefault(s => s.ServiceRoute == resolver.ServiceRoute);

                if (service == null)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }

                var method = service.MethodMappings.SingleOrDefault(m => m.MethodRoute == resolver.MethodRoute && m.HttpMethodType == httpMethod)
                             ?? service.MethodMappings.SingleOrDefault(m => m.HttpMethodType == httpMethod);

                if (method == null || method.HttpMethodType != httpMethod)
                {
                    response.StatusCode = HttpStatusCode.NotFound;
                    return response;
                }

                var methodParameters = method.Method.GetParameters();

                var jsonSerializer = ServiceProvider.Provider.GetOrCreate<IJsonSerializer>();

                object methodParamValue = null;
                if (methodParameters.Any())
                {
                    var methodParam = method.Method.GetParameters()[0];
                    if (methodParam.ParameterType.IsValueType || methodParam.ParameterType == typeof (string))
                    {
                        if (resolver.DefaultParam != null)
                        {
                            methodParamValue = Convert.ChangeType(resolver.DefaultParam, methodParam.ParameterType);
                        }
                    }
                    else
                    {
                        var requestBody = request.Content.ReadAsStringAsync().Result;
                        methodParamValue = jsonSerializer.Deserialize(requestBody, methodParam.ParameterType);
                    }
                }

                var serviceInstance = ServiceProvider.Provider.GetOrCreate(service.ServiceType);

                var result = method.Method.Invoke(serviceInstance, new[] { methodParamValue });

                var json = jsonSerializer.Serialize(result);

                response.Content = new StringContent(json, Encoding.UTF8, "text/plain");

                response.StatusCode = HttpStatusCode.OK;

                return response;
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}