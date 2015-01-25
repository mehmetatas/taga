namespace Taga.Core.Validation
{
    public interface IValidationRule
    {
        bool Execute(object obj);
    }
}
