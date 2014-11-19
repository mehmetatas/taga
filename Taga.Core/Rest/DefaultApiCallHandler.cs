using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using Taga.Core.IoC;
using Taga.Core.Json;

namespace Taga.Core.Rest
{
    public class DefaultApiCallHandler : IApiCallHandler
    {
        private static IJsonSerializer _json;
        private static IJsonSerializer Json
        {
            get { return _json ?? (_json = ServiceProvider.Provider.GetOrCreate<IJsonSerializer>()); }
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

            var result = InvokeAction(request, route, parameterValues);

            SetResponse(response, result);
        }

        private static Route ResolveRoute(HttpRequestMessage request)
        {
            var resolver = new RouteResolver(request);

            var service = ServiceConfig.Current.ServiceMappings.SingleOrDefault(s => s.ServiceRoute == resolver.ServiceRoute);

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

        private static object[] ResolveParameters(HttpRequestMessage request, Route route)
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
                var value = Json.Deserialize(requestBody, parameters[0].ParameterType);
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

        private static object InvokeAction(HttpRequestMessage request, Route route, object[] parameters)
        {
            var methodInfo = route.Method.Method;
            var interceptor = ServiceProvider.Provider.GetOrCreate<IActionInterceptor>();
            var serviceInstance = ServiceProvider.Provider.GetOrCreate(route.Service.ServiceType);
            var context = new HttpRequestContext(request);

            try
            {
                interceptor.BeforeCall(context, methodInfo, parameters);
                var result = methodInfo.Invoke(serviceInstance, parameters);
                interceptor.AfterCall(context, methodInfo, parameters, result);
                return result;
            }
            catch (TargetInvocationException tie)
            {
                return interceptor.OnException(context, methodInfo, parameters, tie.InnerException ?? tie);
            }
            catch (Exception ex)
            {
                return interceptor.OnException(context, methodInfo, parameters, ex);
            }
        }

        private static void SetResponse(HttpResponseMessage response, object result)
        {
            var json = result == null
                ? String.Empty
                : Json.Serialize(result);

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
