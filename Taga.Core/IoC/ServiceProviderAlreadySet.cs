using System;
using Taga.Core.Exceptions;

namespace Taga.Core.IoC
{
    public class ServiceProviderAlreadySetException : CoreException
    {
        public ServiceProviderAlreadySetException(Type providerType)
            : base("Service provier can be set once and it already has been set to {0}", providerType)
        {
        }
    }
}
