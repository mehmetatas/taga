using Taga.Framework.Exceptions;

namespace Taga.Framework.Validation
{
    public class ValidationResult
    {
        internal static readonly ValidationResult Successful = new ValidationResult();

        internal static ValidationResult Failed(Error error)
        {
            return new ValidationResult(error);
        }

        private ValidationResult(Error error)
        {
            Error = error;
        }

        private ValidationResult()
        {

        }

        public Error Error { get; }

        public bool IsValid => Error == null;
    }
}