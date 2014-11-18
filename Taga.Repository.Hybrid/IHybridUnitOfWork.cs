using Taga.Core.Repository.Command;
using Taga.Core.Repository.Hybrid;

namespace Taga.Repository.Hybrid
{
    interface IHybridUnitOfWork
    {
        void Insert(object entity);

        void Update(object entity);

        void Delete(object entity);

        void NonQuery(ICommand command);

        IHybridAdapter Adapter { get; }
    }
}