using System;

namespace Taga.Core.Validation.ValidationRules
{
    public class StringNotEmptyRule : IValidationRule
    {
        public bool Execute(object obj)
        {
            var str = obj as string;
            if (str == null)
            {
                return false;
            }
            return !String.IsNullOrWhiteSpace(str);
        }
    }
}