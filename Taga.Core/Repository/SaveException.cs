using Taga.Core.Exceptions;

namespace Taga.Core.Repository
{
    public class SaveException : CoreException
    {
        public SaveException()
            : base("Save extension method of IRepository can only be called for tables which have one auto-increment identity column!")
        {
        }
    }
}
