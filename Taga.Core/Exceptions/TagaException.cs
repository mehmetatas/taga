using System;

namespace Taga.Core.Exceptions
{
    public abstract class TagaException : Exception
    {
        protected TagaException(string format, params object[] args)
            : base(String.Format(format, args))
        {

        }

        protected TagaException(Exception innerException, string format, params object[] args)
            : base(String.Format(format, args), innerException)
        {

        }
    }
}
