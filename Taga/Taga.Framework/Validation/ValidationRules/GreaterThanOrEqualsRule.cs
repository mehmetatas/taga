using System;

namespace Taga.Framework.Validation.ValidationRules
{
    public class GreaterThanOrEqualsRule<T> : ComparisonRule<T> where T : IComparable<T>
    {
        public GreaterThanOrEqualsRule(T min)
            : base(min)
        {

        }

        protected override bool IsValid(int comparisonResult)
        {
            return comparisonResult != -1;
        }

        protected override bool InvalidParamResult => false;
    }
}