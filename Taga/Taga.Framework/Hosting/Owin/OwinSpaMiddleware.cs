using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Taga.Framework.Hosting.Owin
{
    public class OwinSpaMiddleware : OwinMiddleware
    {
        private static readonly object LockObj = new object();

        private readonly string _defaultFilePath;
        private static string _indexHtml;

        public OwinSpaMiddleware(OwinMiddleware next, string defaultFilePath)
            : base(next)
        {
            _defaultFilePath = defaultFilePath;
        }

        public override async Task Invoke(IOwinContext context)
        {
            if (_indexHtml == null)
            {
                lock (LockObj)
                {
                    if (_indexHtml == null)
                    {
                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _defaultFilePath);
                        _indexHtml = File.ReadAllText(path);
                    }
                }
            }

            context.Response.StatusCode = 200;
            await context.Response.WriteAsync(_indexHtml);
        }
    }
}