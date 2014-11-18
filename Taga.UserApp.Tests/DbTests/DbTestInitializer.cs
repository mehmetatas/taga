using Taga.Core.IoC;
using Taga.Core.Repository;
using Taga.Core.Repository.Mapping;
using Taga.IoC.Ninject;
using Taga.UserApp.Core.Database;
using Taga.UserApp.Core.Model.Database;
using Taga.UserApp.Core.Repository;
using Taga.UserApp.Repository;

namespace Taga.UserApp.Tests.DbTests
{
    public class DbTestInitializer
    {
        public static void Initialize(DbSystem dbSystem)
        {
            ServiceProvider.Init(new NinjectServiceProvider());

            RegisterRepositories();

            ServiceProvider.Provider.RegisterSingleton<IPropertyFilter>(new UserAppPropertyFilter());
            ServiceProvider.Provider.RegisterSingleton<IMappingProvider>(new MappingProvider());

            InitMappings(dbSystem);
        }

        private static void InitMappings(DbSystem dbSystem)
        {
            var dbMapping = DatabaseMapping.For(dbSystem)
                .Map<User>()
                .Map<Role>()
                .Map<Permission>()
                .Map<UserRole>(ur => ur.UserId, ur => ur.RoleId)
                .Map<RolePermission>(rp => rp.RoleId, rp => rp.PermissionId)
                .Map<Category>()
                .Map<Post>()
                .Map<Tag>()
                .Map<PostTag>(pt => pt.PostId, pt => pt.TagId)
                .Map<TagPost>(pt => pt.TagId, pt => pt.PostId);

            var mappingProv = ServiceProvider.Provider.GetOrCreate<IMappingProvider>();

            mappingProv.SetDatabaseMapping(dbMapping);
        }

        private static void RegisterRepositories()
        {
            var prov = ServiceProvider.Provider;

            prov.Register<IUserRepository, UserRepository>();
            prov.Register<IAuthorizationRepository, AuthorizationRepository>();
            prov.Register<IPostRepository, PostRepository>();
        }

        public static void ClearDb()
        {
            using (var uow = Db.ReadWrite())
            {
                var repo = ServiceProvider.Provider.GetOrCreate<IRepository>();
                repo.ExecSp("truncate_all");
                uow.Save();
            }
        }
    }
}
