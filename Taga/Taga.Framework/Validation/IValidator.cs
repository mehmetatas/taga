
namespace Taga.Framework.Validation
{
    public interface IValidator
    {
        ValidationResult Validate(object instance);
    }
}
