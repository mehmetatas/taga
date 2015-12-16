namespace Taga.Framework.Validation.ValidationRules
{
    public class NotNullRule : IValidationRule
    {
        public bool Execute(object obj)
        {
            return obj != null;
        }
    }
}
