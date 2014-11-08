using Taga.SimpLinq.QueryBuilder;

namespace Taga.Core.Repository.SimpLinq
{
    public interface ISimpLinqResolver
    {
        ISqlCommand Resolve(ISelectQuery query);
    }
}