using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Taga.Core.Validation
{
    public abstract class Validator<T> : IValidator
    {
        private readonly List<IValidator> _validators;
        private readonly IValidationRuleBuilder<T> _ruleBuilder;

        protected Validator()
        {
            _ruleBuilder = new ValidationRuleBuilder<T>();
            BuildRules();
            _validators = _ruleBuilder.Build();
        }

        protected PropertyValidatorBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> propExpression)
        {
            return _ruleBuilder.RuleFor(propExpression);
        }

        ValidationResult IValidator.Validate(object obj)
        {
            foreach (var validator in _validators)
            {
                var res = validator.Validate(obj);
                if (!res.IsValid)
                {
                    return res;
                }
            }
            return ValidationResult.Successful;
        }

        protected abstract void BuildRules();
    }
}