using System.Net.Http;

namespace Taga.Core.Rest
{
    public interface IApiCallHandler
    {
        void Handle(HttpRequestMessage request, HttpResponseMessage response);
    }
}
