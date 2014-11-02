using System;
using Taga.Core.Exceptions;

namespace Taga.Core.IoC
{
    public class InvalidSingletonTypeException : CoreException
    {
        public InvalidSingletonTypeException(Type expected, Type found) 
            : base("Invalid singleton type. Expected: {0}, Found: {1}", expected, found)
        {
        }
    }
}
