namespace Taga.Core.Validation.ValidationRules
{
    public class StringLengthRule : IValidationRule
    {
        private readonly int _min;
        private readonly int _max;

        public StringLengthRule(int min, int max)
        {
            _min = min;
            _max = max;
        }

        public bool Execute(object obj)
        {
            var str = obj as string;
            if (str == null)
            {
                return false;
            }
            str = str.Trim();
            return _min <= str.Length && str.Length <= _max;
        }
    }
}