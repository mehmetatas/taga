using System;

namespace Taga.Core.Validation.ValidationRules
{
    public abstract class ComparisonRule<T> : IValidationRule where T : IComparable<T>
    {
        private readonly T _refValue;

        protected ComparisonRule(T refValue)
        {
            _refValue = refValue;
        }

        public virtual bool Execute(object obj)
        {
            if (!(obj is T))
            {
                obj = ValidationUtils.EnsureType(obj, typeof(T));
                if (obj == null)
                {
                    return InvalidParamResult;
                }
            }

            var val = (T)obj;
            return IsValid(val.CompareTo(_refValue));
        }

        protected abstract bool InvalidParamResult { get; }

        protected abstract bool IsValid(int comparisonResult);
    }
}
