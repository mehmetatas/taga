
namespace Taga.Framework.Exceptions
{
    public class Errors
    {
        public static readonly Error Unknown = new Error(-1, "Error_Unknown");

        public static readonly Error F_RouteResolvingError = new Error(1, "F_RouteResolvingError");
        public static readonly Error F_NullRequest = new Error(2, "F_NullRequest");
    }
}
