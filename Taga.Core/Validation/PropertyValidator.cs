using System.Reflection;

namespace Taga.Core.Validation
{
    public class PropertyValidator : IValidator
    {
        private readonly PropertyInfo[] _propInfoChain;
        private readonly IValidationRule _rule;
        private readonly object _error;

        public PropertyValidator(PropertyInfo[] propInfoChain, IValidationRule rule, object error)
        {
            _propInfoChain = propInfoChain;
            _rule = rule;
            _error = error;
        }

        public ValidationResult Validate(object instance)
        {
            var value = instance;
            foreach (var propInf in _propInfoChain)
            {
                value = propInf.GetValue(value);
            }

            if (_rule.Execute(value))
            {
                return ValidationResult.Successful;
            }
            return ValidationResult.Failed(_error);
        }
    }
}