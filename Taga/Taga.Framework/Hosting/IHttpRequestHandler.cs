using System.Threading.Tasks;

namespace Taga.Framework.Hosting
{
    public interface IHttpRequestHandler
    {
        Task Handle(IHttpRequest httpRequest, IHttpResponse httpResponse);
    }
}
