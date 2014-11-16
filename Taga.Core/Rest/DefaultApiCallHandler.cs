using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using Taga.Core.IoC;
using Taga.Core.Json;

namespace Taga.Core.Rest
{
    public class DefaultApiCallHandler : IApiCallHandler
    {
        private readonly ServiceConfig _serviceConfig;
        private readonly IJsonSerializer _jsonSerializer;

        public DefaultApiCallHandler()
        {
            _serviceConfig = ServiceConfig.Current;
            _jsonSerializer = ServiceProvider.Provider.GetOrCreate<IJsonSerializer>();
        }

        public void Handle(HttpRequestMessage request, HttpResponseMessage response)
        {
            var route = ResolveRoute(request);

            if (route == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return;
            }

            var parameterValues = ResolveParameters(request, route);

            var result = InvokeAction(route, parameterValues);

            SetResponse(response, result);
        }

        private Route ResolveRoute(HttpRequestMessage request)
        {
            var resolver = new RouteResolver(request);

            var service = _serviceConfig.ServiceMappings.SingleOrDefault(s => s.ServiceRoute == resolver.ServiceRoute);

            if (service == null)
            {
                return null;
            }

            var method = service.MethodMappings.SingleOrDefault(m => m.MethodRoute == resolver.MethodRoute && m.HttpMethodType == resolver.HttpMethodType);

            if (method == null)
            {
                return null;
            }

            return new Route
            {
                Service = service,
                Method = method,
                Resolver = resolver
            };
        }

        private object[] ResolveParameters(HttpRequestMessage request, Route route)
        {
            var parameters = route.Method.Method.GetParameters();

            if (parameters.Length == 0)
            {
                return null;
            }

            var isSingleComplexTypedParameter = parameters.Length == 1 && parameters[0].ParameterType.IsClass &&
                                                parameters[0].ParameterType != typeof(string);

            if (isSingleComplexTypedParameter)
            {
                var requestBody = request.Content.ReadAsStringAsync().Result;
                var value = _jsonSerializer.Deserialize(requestBody, parameters[0].ParameterType);
                return new[] { value };
            }

            var resolver = route.Resolver;

            if (parameters.Length == 1 && resolver.DefaultParam != null)
            {
                return new[] { Convert.ChangeType(resolver.DefaultParam, parameters[0].ParameterType) };
            }

            var parameterValues = new object[parameters.Length];

            foreach (var parameter in parameters)
            {
                var found = false;
                foreach (var pair in request.GetQueryNameValuePairs())
                {
                    if (pair.Key == parameter.Name)
                    {
                        parameterValues[parameter.Position] = Convert.ChangeType(pair.Value, parameter.ParameterType);
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    parameterValues[parameter.Position] = parameter.ParameterType.IsValueType
                        ? Activator.CreateInstance(parameter.ParameterType)
                        : null;
                }
            }

            return parameterValues;
        }

        private static object InvokeAction(Route route, object[] parameters)
        {
            var methodInfo = route.Method.Method;
            var interceptor = ServiceProvider.Provider.GetOrCreate<IActionInterceptor>();
            var serviceInstance = ServiceProvider.Provider.GetOrCreate(route.Service.ServiceType);

            try
            {
                interceptor.BeforeCall(methodInfo, parameters);
                var result = methodInfo.Invoke(serviceInstance, parameters);
                interceptor.AfterCall(methodInfo, parameters, result);
                return result;
            }
            catch (Exception ex)
            {
                return interceptor.OnException(methodInfo, parameters, ex);
            }
        }

        private void SetResponse(HttpResponseMessage response, object result)
        {
            var json = result == null
                ? String.Empty
                : _jsonSerializer.Serialize(result);

            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            response.StatusCode = HttpStatusCode.OK;
        }

        private class Route
        {
            public ServiceMapping Service { get; set; }
            public MethodMapping Method { get; set; }
            public RouteResolver Resolver { get; set; }
        }
    }
}
