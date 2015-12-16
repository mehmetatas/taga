using Microsoft.Owin;

namespace Taga.Framework.Hosting.Owin
{
    class OwinHttpResponse : IHttpResponse
    {
        private readonly IOwinResponse _response;

        public OwinHttpResponse(IOwinResponse response)
        {
            _response = response;
        }

        public void SetHeader(string key, string value)
        {
            _response.Headers[key] = value;
        }

        public void SetContent(string json)
        {
            _response.StatusCode = 200;
            _response.ContentType = "application/json";
            _response.Write(json);
        }
    }
}