using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Taga.Core.Validation
{
    public class ValidationRuleBuilder<T> : IValidationRuleBuilder<T>
    {
        private readonly List<IValidator> _validators = new List<IValidator>();
        private IValidatorBuilder _current;

        public PropertyValidatorBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> propExpression)
        {
            AddCurrent();

            var propBuilder = new PropertyValidatorBuilder<T, TProperty>(propExpression);
            _current = propBuilder;
            return propBuilder;
        }

        public List<IValidator> Build()
        {
            AddCurrent();
            return _validators;
        }

        private void AddCurrent()
        {
            if (_current != null)
            {
                _validators.Add(_current.Build());
            }
        }
    }
}