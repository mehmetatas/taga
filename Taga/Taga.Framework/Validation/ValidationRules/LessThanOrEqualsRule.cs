using System;

namespace Taga.Framework.Validation.ValidationRules
{
    public class LessThanOrEqualsRule<T> : ComparisonRule<T> where T : IComparable<T>
    {
        public LessThanOrEqualsRule(T max)
            : base(max)
        {

        }

        protected override bool IsValid(int comparisonResult)
        {
            return comparisonResult != 1;
        }

        protected override bool InvalidParamResult => false;
    }
}