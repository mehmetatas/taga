using System;
using System.Threading.Tasks;
using Taga.Framework.Exceptions;
using Taga.Framework.Json;

namespace Taga.Framework.Hosting.Impl
{
    public class HttpRequestHandler : IHttpRequestHandler
    {
        private readonly IJsonSerializer _json;
        private readonly IRouteResolver _routeResolver;
        private readonly IParameterResolver _parameterResolver;
        private readonly IActionInvoker _invoker;

        public HttpRequestHandler(IJsonSerializer json, IRouteResolver routeResolver, IParameterResolver parameterResolver, IActionInvoker invoker)
        {
            _json = json;
            _routeResolver = routeResolver;
            _parameterResolver = parameterResolver;
            _invoker = invoker;
        }

        public Task Handle(IHttpRequest httpRequest, IHttpResponse httpResponse)
        {
            return Task.Run(() =>
            {
                try
                {
                    var routeContext = _routeResolver.Resolve(httpRequest);

                    _parameterResolver.Resolve(routeContext);

                    _invoker.InvokeAction(routeContext);

                    WriteResponse(httpResponse, routeContext.ReturnValue);
                }
                catch (Exception ex)
                {
                    var err = ex as Error ?? Errors.Unknown;
                    WriteResponse(httpResponse, Response.Error(err));
                }
            });
        }

        private void WriteResponse(IHttpResponse httpResponse, object result)
        {
            var json = result == null
                ? string.Empty
                : _json.Serialize(result);

            httpResponse.SetContent(json);
        }
    }
}
