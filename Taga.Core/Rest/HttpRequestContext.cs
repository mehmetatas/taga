using System.Linq;
using System.Net.Http;

namespace Taga.Core.Rest
{
    public class HttpRequestContext : IRequestContext
    {
        private readonly HttpRequestMessage _request;

        public HttpRequestContext(HttpRequestMessage request)
        {
            _request = request;
        }

        public string GetHeader(string name)
        {
            return _request.Headers.Contains(name)
                ? _request.Headers.GetValues(name).FirstOrDefault()
                : null;
        }
    }
}
