using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Taga.Core.IoC;

namespace Taga.Core.Rest
{
    public class GenericApiHandler : DelegatingHandler
    {
        public GenericApiHandler(HttpConfiguration httpConfig)
        {
            InnerHandler = new HttpControllerDispatcher(httpConfig);
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(responseToCompleteTask =>
            {
                var response = responseToCompleteTask.Result;

                var handler = ServiceProvider.Provider.GetOrCreate<IApiCallHandler>();
                handler.Handle(request, response);

                return response;
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}