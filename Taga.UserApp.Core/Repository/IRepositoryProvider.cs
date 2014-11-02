
namespace Taga.UserApp.Core.Repository
{
    public interface IRepositoryProvider
    {
        T GetRepository<T>() where T : IUserAppRepository;
    }
}
