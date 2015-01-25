using System.Collections.Generic;

namespace Taga.Core.Validation
{
    public class CompositeValidator : IValidator
    {
        private readonly List<IValidator> _validators;

        public CompositeValidator()
        {
            _validators = new List<IValidator>();
        }

        public void AddValidator(IValidator validator)
        {
            _validators.Add(validator);
        }

        public ValidationResult Validate(object obj)
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
    }
}
