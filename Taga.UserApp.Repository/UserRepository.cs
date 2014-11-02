using System.Linq;
using Taga.Core.Repository;
using Taga.UserApp.Core.Model.Database;
using Taga.UserApp.Core.Repository;

namespace Taga.UserApp.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly IRepository _repository;

        public UserRepository(IRepository repository)
        {
            _repository = repository;
        }

        public virtual void Save(User user)
        {
            _repository.Save(user);
        }

        public virtual User Get(long id)
        {
            return _repository.Select<User>()
                .SingleOrDefault(u => u.Id == id);
        }

        public virtual void Delete(User user)
        {
            _repository.Delete(user);
        }
    }
}
