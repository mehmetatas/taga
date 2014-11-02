using Taga.UserApp.Core.Model.Database;

namespace Taga.UserApp.Core.Repository
{
    public interface IUserRepository : IUserAppRepository
    {
        void Save(User user);

        User Get(long id);
        
        void Delete(User user);
    }
}
