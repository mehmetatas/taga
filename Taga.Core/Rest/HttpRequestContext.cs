using System.Linq;
using System.Net.Http;

namespace Taga.Core.Rest
{
    public class HttpRequestContext : IRequestContext
    {
        private readonly HttpRequestMessage _request;
        private readonly HttpResponseMessage _response;

        public HttpRequestContext(HttpRequestMessage request, HttpResponseMessage response, bool rollbackOnError)
        {
            _request = request;
            _response = response;
            RollbackOnError = rollbackOnError;
        }

        public string GetHeader(string name)
        {
            return _request.Headers.Contains(name)
                ? _request.Headers.GetValues(name).FirstOrDefault()
                : null;
        }

        public void SetHeader(string name, string value)
        {
            _response.Headers.Add(name, value);
        }


        public bool RollbackOnError { get; private set; }
    }
}
