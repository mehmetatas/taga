using System;
using System.Linq.Expressions;
using Taga.Framework.Exceptions;

namespace Taga.Framework.Validation
{
    public class PropertyValidatorBuilder<TEntity, TProperty> : IPropertyValidatorBuilder<TEntity, TProperty>
    {
        private readonly PropertyValidator _validator;

        public PropertyValidatorBuilder(Expression<Func<TEntity, TProperty>> propExpression)
        {
            var resolver = new PropertyChainResolver(propExpression);
            var propChain = resolver.Resolve();
            _validator = new PropertyValidator(propChain);
        }

        public IPropertyValidatorBuilder<TEntity, TProperty> AddRule(IValidationRule rule, Error error)
        {
            _validator.SetRule(rule).SetError(error);
            return this;
        }

        public IValidator Build()
        {
            return _validator;
        }
    }
}