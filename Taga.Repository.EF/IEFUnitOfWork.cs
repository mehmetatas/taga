using System.Data.Entity;

namespace Taga.Repository.EF
{
    internal interface IEFUnitOfWork
    {
        DbContext DbContext { get; }
    }
}
