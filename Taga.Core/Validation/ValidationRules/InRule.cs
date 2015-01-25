using System.Linq;

namespace Taga.Core.Validation.ValidationRules
{
    public class InRule<T> : IValidationRule
    {
        private readonly T[] _values;

        public InRule(params T[] values)
        {
            _values = values;
        }

        public bool Execute(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            return _values.Any(value => obj.Equals(value));
        }
    }
}
