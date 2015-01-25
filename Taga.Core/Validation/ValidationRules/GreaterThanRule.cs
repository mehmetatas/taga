using System;

namespace Taga.Core.Validation.ValidationRules
{
    public class GreaterThanRule<T> : ComparisonRule<T> where T : IComparable<T>
    {
        public GreaterThanRule(T min)
            : base(min)
        {

        }

        protected override bool IsValid(int comparisonResult)
        {
            return comparisonResult == 1;
        }

        protected override bool InvalidParamResult
        {
            get { return false; }
        }
    }
}
