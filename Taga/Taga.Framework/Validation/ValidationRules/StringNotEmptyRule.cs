
namespace Taga.Framework.Validation.ValidationRules
{
    public class StringNotEmptyRule : IValidationRule
    {
        public bool Execute(object obj)
        {
            var str = obj as string;
            return !string.IsNullOrWhiteSpace(str);
        }
    }
}