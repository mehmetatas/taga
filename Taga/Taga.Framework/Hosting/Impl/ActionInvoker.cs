using System;
using System.Reflection;
using Taga.Framework.Exceptions;
using Taga.Framework.IoC;
using Taga.Framework.Logging;
using Taga.Framework.Logging.Impl;

namespace Taga.Framework.Hosting.Impl
{
    public class ActionInvoker : IActionInvoker
    {
        private readonly IActionInterceptorBuilder _interceptorBuilder;
        private readonly ILogger _logger;

        public ActionInvoker(IActionInterceptorBuilder interceptorBuilder, ILogger logger)
        {
            _interceptorBuilder = interceptorBuilder;
            _logger = logger;
        }

        public virtual void InvokeAction(RouteContext ctx)
        {
            var serviceInstance = DependencyContainer.Current.Resolve(ctx.Service.ServiceType);

            using (var interceptor = _interceptorBuilder.Build(ctx))
            {
                try
                {
                    ctx.ReturnValue = interceptor.BeforeCall(ctx) ??
                                      ctx.Method.Method.Invoke(serviceInstance, ctx.Parameters);

                    interceptor.AfterCall(ctx);
                }
                catch (Exception ex)
                {
                    ctx.Exception = ex;
                    if (!HandleException(interceptor, ctx))
                    {
                        throw;
                    }
                }
            }
        }

        private bool HandleException(IActionInterceptor interceptor, RouteContext ctx)
        {
            var result = interceptor.OnException(ctx);
            if (result != null)
            {
                ctx.ReturnValue = result;
            }

            var ex = ctx.Exception;
            while (ex is TargetInvocationException && ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            _logger.Log(Log.Exception(ex));

            var knownEx = ex as Error;
            if (knownEx == null)
            {
                return false;
            }

            ctx.ReturnValue = Response.Error(knownEx);
            return true;
        }
    }
}