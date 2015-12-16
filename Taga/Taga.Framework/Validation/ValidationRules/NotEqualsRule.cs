using System;

namespace Taga.Framework.Validation.ValidationRules
{
    public class NotEqualsRule<T> : ComparisonRule<T> where T : IComparable<T>
    {
        public NotEqualsRule(T notExpected)
            : base(notExpected)
        {

        }

        protected override bool IsValid(int comparisonResult)
        {
            return comparisonResult != 0;
        }

        protected override bool InvalidParamResult => false;
    }

    public class NotEqualsRule : IValidationRule
    {
        private readonly object _notExpected;

        public NotEqualsRule(object notExpected)
        {
            _notExpected = notExpected;
        }

        public bool Execute(object obj)
        {
            if (obj == null)
            {
                return _notExpected != null;
            }
            return !obj.Equals(_notExpected);
        }
    }
}
