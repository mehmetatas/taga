
namespace Taga.Framework.Hosting
{
    public interface IParameterResolver
    {
        void Resolve(RouteContext ctx);
    }
}
