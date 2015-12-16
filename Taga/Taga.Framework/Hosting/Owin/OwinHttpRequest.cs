using System;
using System.IO;
using Microsoft.Owin;
using Taga.Framework.Hosting.Configuration;

namespace Taga.Framework.Hosting.Owin
{
    class OwinHttpRequest : IHttpRequest
    {
        private readonly IOwinRequest _request;
        private string _content;

        public OwinHttpRequest(IOwinRequest request)
        {
            _request = request;
        }

        public Uri Uri => _request.Uri;

        public string Content
        {
            get
            {
                if (_content == null)
                {
                    using (var sr = new StreamReader(_request.Body))
                    {
                        _content = sr.ReadToEnd();
                    }
                }

                return _content;
            }
        }

        public HttpMethod Method
        {
            get
            {
                switch (_request.Method)
                {
                    case "POST":
                        return HttpMethod.Post;
                    case "GET":
                        return HttpMethod.Get;
                    case "PUT":
                        return HttpMethod.Put;
                    case "DELETE":
                        return HttpMethod.Delete;
                    default:
                        throw new NotSupportedException("Unsupported http method: " + _request.Method);
                }
            }
        }

        public string GetHeader(string key)
        {
            return _request.Headers.ContainsKey(key)
                ? _request.Headers[key]
                : null;
        }

        public string GetParam(string key)
        {
            return _request.Query[key];
        }
    }
}
