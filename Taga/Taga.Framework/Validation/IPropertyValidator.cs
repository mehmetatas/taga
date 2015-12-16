using System.Reflection;
using Taga.Framework.Exceptions;

namespace Taga.Framework.Validation
{
    public interface IPropertyValidator
    {
        PropertyInfo[] PropertyInfoChain { get; }
        IValidationRule Rule { get; }
        Error Error { get; }
    }
}