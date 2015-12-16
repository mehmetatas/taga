using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Taga.Framework.Hosting.Owin
{
    public class OwinSpaMiddleware : OwinMiddleware
    {
        private readonly string _defaultFilePath;
        private string _indexHtml;

        public OwinSpaMiddleware(OwinMiddleware next, string defaultFilePath)
            : base(next)
        {
            _defaultFilePath = defaultFilePath;
        }

        public override async Task Invoke(IOwinContext context)
        {
            await Next.Invoke(context);

            if (context.Response.StatusCode == 404)
            {
                if (_indexHtml == null)
                {
                    lock (this)
                    {
                        if (_indexHtml == null)
                        {
                            var path = Path.Combine(Environment.CurrentDirectory, _defaultFilePath);
                            _indexHtml = File.ReadAllText(path);
                        }
                    }
                }

                context.Response.StatusCode = 200;
                context.Response.Write(_indexHtml);
            }
        }
    }
}