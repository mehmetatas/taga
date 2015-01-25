namespace Taga.Core.Validation.ValidationRules
{
    public class NullRule : IValidationRule
    {
        public bool Execute(object obj)
        {
            return obj == null;
        }
    }
}