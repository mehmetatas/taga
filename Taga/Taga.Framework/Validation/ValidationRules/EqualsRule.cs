using System;

namespace Taga.Framework.Validation.ValidationRules
{
    public class EqualsRule<T> : ComparisonRule<T> where T : IComparable<T>
    {
        public EqualsRule(T expected)
            : base(expected)
        {

        }

        protected override bool IsValid(int comparisonResult)
        {
            return comparisonResult == 0;
        }

        protected override bool InvalidParamResult => false;
    }

    public class EqualsRule : IValidationRule
    {
        private readonly object _expected;

        public EqualsRule(object expected)
        {
            _expected = expected;
        }

        public bool Execute(object obj)
        {
            if (obj == null)
            {
                return _expected == null;
            }
            return obj.Equals(_expected);
        }
    }
}