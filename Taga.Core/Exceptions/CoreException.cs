using System;

namespace Taga.Core.Exceptions
{
    public abstract class CoreException : TagaException
    {
        protected CoreException(string format, params object[] args)
            : base(format, args)
        {
        }

        protected CoreException(Exception innerException, string format, params object[] args)
            : base(innerException, format, args)
        {
        }
    }
}
