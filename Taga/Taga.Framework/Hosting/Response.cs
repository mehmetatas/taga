using Taga.Framework.Exceptions;

namespace Taga.Framework.Hosting
{
    public class Response
    {
        public int ResponseCode { get; private set; }
        public string ResponseMessage { get; private set; }
        public object Data { get; private set; }

        private Response()
        {

        }

        public static Response Error(Error error)
        {
            return new Response
            {
                ResponseCode = error.Code,
                ResponseMessage = error.Message
            };
        }

        public static readonly Response Success = new Response();

        public Response WithMessage(string message = null)
        {
            var resp = EnsureNew();
            resp.ResponseMessage = message;
            return resp;
        }

        public Response WithData(object data = null)
        {
            var resp = EnsureNew();
            resp.Data = data;
            return resp;
        }

        private Response EnsureNew()
        {
            if (this == Success)
            {
                return new Response
                {
                    Data = Data,
                    ResponseCode = ResponseCode,
                    ResponseMessage = ResponseMessage
                };
            }
            return this;
        }
    }
}
