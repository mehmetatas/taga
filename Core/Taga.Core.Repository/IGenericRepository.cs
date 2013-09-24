using Taga.Core.Model;

namespace Taga.Core.Repository
{
    public interface IGenericRepository<T> where T : class, IEntity
    {
        IRepository<T> Repository { get; set; }
    }
}
