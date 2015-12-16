using System;
using Taga.Framework.Hosting.Configuration;

namespace Taga.Framework.Hosting
{
    public interface IHttpRequest
    {
        Uri Uri { get; }
        string Content { get; }
        HttpMethod Method { get; }
        string GetHeader(string key);
        string GetParam(string key);
    }
}
