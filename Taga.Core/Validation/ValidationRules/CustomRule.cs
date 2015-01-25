using System;

namespace Taga.Core.Validation.ValidationRules
{
    public class CustomRule<T> : IValidationRule
    {
        private readonly Func<T, bool> _customValidationFunction;

        public CustomRule(Func<T, bool> customValidationFunction)
        {
            _customValidationFunction = customValidationFunction;
        }

        public bool Execute(object obj)
        {
            return _customValidationFunction((T)obj);
        }
    }
}
