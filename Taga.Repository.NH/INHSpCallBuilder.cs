using Taga.Core.Repository.Command;

namespace Taga.Repository.NH
{
    public interface INHSpCallBuilder
    {
        string BuildSpCall(ICommand cmd);
    }
}
