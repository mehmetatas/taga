using System.Data;
using Taga.Core.Repository;
using Taga.Core.Repository.Hybrid;

namespace Taga.Repository.Hybrid
{
    interface IHybridUnitOfWork
    {
        void Insert(object entity);

        void Update(object entity);

        void Delete(object entity);

        IHybridQueryProvider QueryProvider { get; }

        IDbCommand CreateCommand();
    }
}