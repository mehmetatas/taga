namespace Taga.Framework.Hosting
{
    public interface IRouteResolver
    {
        RouteContext Resolve(IHttpRequest httpRequest);
    }
}
