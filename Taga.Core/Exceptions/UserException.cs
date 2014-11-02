using System;

namespace Taga.Core.Exceptions
{
    public abstract class UserException : TagaException
    {
        protected UserException(string format, params object[] args)
            : base(format, args)
        {
        }

        protected UserException(Exception innerException, string format, params object[] args)
            : base(innerException, format, args)
        {
        }
    }
}
