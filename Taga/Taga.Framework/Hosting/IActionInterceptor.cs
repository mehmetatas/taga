using System;

namespace Taga.Framework.Hosting
{
    public interface IActionInterceptor : IDisposable
    {
        object BeforeCall(RouteContext ctx);

        void AfterCall(RouteContext ctx);

        object OnException(RouteContext ctx);
    }
}