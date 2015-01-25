using System;

namespace Taga.Core.Validation.ValidationRules
{
    public class GuidRule : IValidationRule
    {
        public bool Execute(object obj)
        {
            if (obj is Guid)
            {
                return true;
            }

            var str = obj as string;

            if (str == null)
            {
                return false;
            }

            Guid dummy;
            return Guid.TryParse(str, out dummy);
        }
    }
}
