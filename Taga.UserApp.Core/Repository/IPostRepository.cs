using Taga.Core.Repository;
using Taga.UserApp.Core.Model.Database;

namespace Taga.UserApp.Core.Repository
{
    public interface IPostRepository : IUserAppRepository
    {
        void Save(Category category);

        Category GetCategory(long categoryId);

        Category[] GetCategoriesOfUser(long userId);

        void Delete(Category category);

        void Save(Post post);

        Post GetPost(long postId);

        void Delete(Post post);

        IPage<Post> GetPostsOfUser(long userId, int pageIndex = 1, int pageSize = 10);

        IPage<Post> GetPostsOfCategory(long catId, int pageIndex = 1, int pageSize = 10);
    }
}
