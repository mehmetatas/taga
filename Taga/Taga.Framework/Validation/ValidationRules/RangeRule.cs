using System;

namespace Taga.Framework.Validation.ValidationRules
{
    public class RangeRule<T> : IValidationRule where T : IComparable<T>
    {
        private readonly T _min;
        private readonly T _max;

        public RangeRule(T min, T max)
        {
            _min = min;
            _max = max;
        }

        public bool Execute(object obj)
        {
            if (!(obj is T))
            {
                obj = ValidationUtils.EnsureType(obj, typeof(T));
                if (obj == null)
                {
                    return false;   
                }
            }

            var val = (T)obj;
            return val.CompareTo(_min) > -1 && val.CompareTo(_max) < 1;
        }
    }
}