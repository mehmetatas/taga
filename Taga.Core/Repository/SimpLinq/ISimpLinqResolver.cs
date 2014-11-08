using Taga.Core.Repository.Command;
using Taga.SimpLinq.QueryBuilder;

namespace Taga.Core.Repository.SimpLinq
{
    public interface ISimpLinqResolver
    {
        ICommand Resolve(ISelectQuery query);
    }
}