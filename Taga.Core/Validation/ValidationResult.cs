
namespace Taga.Core.Validation
{
    public class ValidationResult
    {
        internal static readonly ValidationResult Successful = new ValidationResult();

        internal static ValidationResult Failed(object error)
        {
            return new ValidationResult(error);
        }

        private ValidationResult(object error)
        {
            Error = error;
        }

        private ValidationResult()
        {

        }

        public object Error { get; private set; }

        public bool IsValid
        {
            get { return Error == null; }
        }
    }
}