using System.Linq;
using System.Net.Http;

namespace Taga.Core.Rest
{
    public class HttpRequestContext : IRequestContext
    {
        private readonly HttpRequestMessage _request;
        private readonly HttpResponseMessage _response;

        public HttpRequestContext(HttpRequestMessage request, HttpResponseMessage response)
        {
            _request = request;
            _response = response;
        }

        public string GetRequestHeader(string name)
        {
            return _request.Headers.Contains(name)
                ? _request.Headers.GetValues(name).FirstOrDefault()
                : null;
        }

        public void SetResponseHeader(string name, string value)
        {
            _response.Headers.Add(name, value);
        }
    }
}
