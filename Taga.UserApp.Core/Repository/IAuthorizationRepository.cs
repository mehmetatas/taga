using System.Collections.Generic;
using System.Linq;
using Taga.Core.Repository;
using Taga.UserApp.Core.Model.Common.Filters;
using Taga.UserApp.Core.Model.Database;

namespace Taga.UserApp.Core.Repository
{
    public interface IAuthorizationRepository : IUserAppRepository
    {
        void Save(Role role);

        void Save(Permission permission);

        void SaveRolesOfUser(long userId, params Role[] roles);

        void SavePermissionsOfRole(long roleId, params Permission[] permissions);

        void Delete(Role role);

        void Delete(Permission permission);

        Role GetRole(long roleId);

        Permission GetPermission(long permissionId);

        IDictionary<long, Permission[]> GetPermissions(params long[] roleIds);

        IDictionary<long, Role[]> GetRoles(params long[] userIds);

        IPage<Role> SearchRoles(RoleFilter filter, int pageIndex = 1, int pageSize = 1000);
    }

    public static class AuthorizationRepositoryExtensions
    {
        public static Role[] GetRolesOfUser(this IAuthorizationRepository repo, long userId)
        {
            return repo.GetRoles(userId).Values.SelectMany(r => r).ToArray();
        }

        public static Permission[] GetPermissionsOfRole(this IAuthorizationRepository repo, long roleId)
        {
            return repo.GetPermissions(roleId).Values.SelectMany(p => p).ToArray();
        }
    }
}
