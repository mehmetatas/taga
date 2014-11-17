using System;

namespace Taga.Core.Exceptions
{
    public abstract class TagaAppException : TagaException
    {
        protected TagaAppException(string format, params object[] args)
            : base(format, args)
        {
        }

        protected TagaAppException(Exception innerException, string format, params object[] args)
            : base(innerException, format, args)
        {
        }
    }
}
