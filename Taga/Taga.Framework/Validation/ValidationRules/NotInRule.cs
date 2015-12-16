using System.Linq;

namespace Taga.Framework.Validation.ValidationRules
{
    public class NotInRule<T> : IValidationRule
    {
        private readonly T[] _values;

        public NotInRule(params T[] values)
        {
            _values = values;
        }

        public bool Execute(object obj)
        {
            if (obj == null)
            {
                return true;
            }

            return _values.All(value => !obj.Equals(value));
        }
    }
}
