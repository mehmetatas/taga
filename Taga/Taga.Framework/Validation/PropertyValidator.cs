using System.Reflection;
using Taga.Framework.Exceptions;

namespace Taga.Framework.Validation
{
    class PropertyValidator : IPropertyValidator, IValidator
    {
        public PropertyInfo[] PropertyInfoChain { get;  }
        public IValidationRule Rule { get; private set; }
        public Error Error { get; private set; }

        public PropertyValidator(PropertyInfo[] propInfoChain)
        {
            PropertyInfoChain = propInfoChain;
        }

        public PropertyValidator SetRule(IValidationRule rule)
        {
            Rule = rule;
            return this;
        }

        public PropertyValidator SetError(Error error)
        {
            Error = error;
            return this;
        }
        

        ValidationResult IValidator.Validate(object instance)
        {
            var value = instance;
            foreach (var propInf in PropertyInfoChain)
            {
                value = propInf.GetValue(value);
            }

            if (Rule.Execute(value))
            {
                return ValidationResult.Successful;
            }
            return ValidationResult.Failed(Error);
        }
    }
}