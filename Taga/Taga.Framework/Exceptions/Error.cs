using System;
using Taga.Framework.Utils;

namespace Taga.Framework.Exceptions
{
    public class Error : Exception
    {
        private readonly static object[] EmptyArgs = new object[0];

        private string _message;
        private object[] _messageArgs = EmptyArgs;

        public Error(int code, string messageCode)
        {
            Code = code;
            MessageCode = messageCode;
        }

        public int Code { get; }
        public string MessageCode { get; }
        
        public override string Message => 
            _message ?? (_message = string.Format(ML.GetValue(MessageCode), _messageArgs));

        public Error WithArgs(params object[] args)
        {
            return new Error(Code, MessageCode)
            {
                _messageArgs = args
            };
        }
    }
}
