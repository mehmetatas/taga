
using Taga.Framework.Exceptions;

namespace Taga.Framework.Validation
{
    public interface IPropertyValidatorBuilder<TEntity, TProperty> : IValidatorBuilder
    {
        IPropertyValidatorBuilder<TEntity, TProperty> AddRule(IValidationRule rule, Error error);
    }
}