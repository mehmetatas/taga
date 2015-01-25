
namespace Taga.Core.Validation
{
    public interface IValidator
    {
        ValidationResult Validate(object instance);
    }
}
