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
            var route = ResolveRoute(request, response);

            if (route == null)
            {
                response.StatusCode = HttpStatusCode.NotFound;
                return;
            }

            var parameterValues = ResolveParameters(route);

            var result = InvokeAction(route, parameterValues);

            SetResponse(response, result);
        }

        private static RouteContext ResolveRoute(HttpRequestMessage request, HttpResponseMessage response)
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

            return new RouteContext
            {
                Service = service,
                Method = method,
                Resolver = resolver,
                Request = request,
                Response = response
            };
        }

        private static object[] ResolveParameters(RouteContext route)
        {
            var parameters = route.Method.Method.GetParameters().OrderBy(p => p.Position).ToArray();

            if (parameters.Length == 0)
            {
                return new object[0];
            }

            var isSingleComplexTypedParameter = parameters.Length == 1 && parameters[0].ParameterType.IsClass &&
                                                parameters[0].ParameterType != typeof(string);

            if (isSingleComplexTypedParameter)
            {
                var paramType = parameters[0].ParameterType;

                if (route.Request.Method == HttpMethod.Post || route.Request.Method == HttpMethod.Put)
                {
                    var requestBody = route.Request.Content.ReadAsStringAsync().Result;
                    var value = Json.Deserialize(requestBody, paramType);
                    return new[] { value };
                }

                var queryStringParams = route.Request.GetQueryNameValuePairs()
                    .Select(kv => new
                    {
                        kv.Key,
                        kv.Value
                    })
                    .ToList();

                var paramValue = Activator.CreateInstance(paramType);

                foreach (var prop in paramType.GetProperties())
                {
                    var queryStringParam = queryStringParams.FirstOrDefault(
                        param => param.Key.Equals(prop.Name, StringComparison.InvariantCultureIgnoreCase));

                    if (queryStringParam != null)
                    {
                        prop.SetValue(paramValue, Convert.ChangeType(queryStringParam.Value, prop.PropertyType));
                    }
                }

                return new[] { paramValue };
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
                foreach (var pair in route.Request.GetQueryNameValuePairs())
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

        private static object InvokeAction(RouteContext route, object[] parameters)
        {
            var methodInfo = route.Method.Method;
            var serviceInstance = ServiceProvider.Provider.GetOrCreate(route.Service.ServiceType);
            var context = new HttpRequestContext(route.Request, route.Response);

            using (var interceptor = ServiceProvider.Provider.GetOrCreate<IActionInterceptor>())
            {
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
        }

        private static void SetResponse(HttpResponseMessage response, object result)
        {
            var json = result == null
                ? String.Empty
                : Json.Serialize(result);

            response.Content = new StringContent(json, Encoding.UTF8, "application/json");

            response.StatusCode = HttpStatusCode.OK;
        }

        private class RouteContext
        {
            public ServiceMapping Service { get; set; }
            public MethodMapping Method { get; set; }
            public RouteResolver Resolver { get; set; }
            public HttpRequestMessage Request { get; set; }
            public HttpResponseMessage Response { get; set; }
        }
    }
}
