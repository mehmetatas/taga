using System;
using System.Collections.Generic;
using System.Linq;
using Taga.Core.Repository;
using Taga.UserApp.Core.Model.Common.Filters;
using Taga.UserApp.Core.Model.Database;
using Taga.UserApp.Core.Repository;

namespace Taga.UserApp.Repository
{
    public class AuthorizationRepository : IAuthorizationRepository
    {
        private readonly IRepository _repository;

        public AuthorizationRepository(IRepository repository)
        {
            _repository = repository;
        }

        public void Save(Role role)
        {
            _repository.Save(role);
        }

        public void Save(Permission permission)
        {
            _repository.Save(permission);
        }

        public void SaveRolesOfUser(long userId, params Role[] roles)
        {
            var existingRoles = this.GetRolesOfUser(userId);

            foreach (var role in existingRoles)
            {
                _repository.Delete(new UserRole
                {
                    RoleId = role.Id,
                    UserId = userId
                });
            }

            foreach (var role in roles)
            {
                _repository.Insert(new UserRole
                {
                    RoleId = role.Id,
                    UserId = userId
                });
            }
        }

        public void SavePermissionsOfRole(long roleId, params Permission[] permissions)
        {
            var existingPermissions = this.GetPermissionsOfRole(roleId);

            foreach (var permission in existingPermissions)
            {
                _repository.Delete(new RolePermission
                {
                    PermissionId = permission.Id,
                    RoleId = roleId
                });
            }

            foreach (var permission in permissions)
            {
                _repository.Insert(new RolePermission
                {
                    PermissionId = permission.Id,
                    RoleId = roleId
                });
            }
        }

        public void Delete(Role role)
        {
            _repository.Delete(role);
        }

        public void Delete(Permission permission)
        {
            _repository.Delete(permission);
        }

        public Role GetRole(long roleId)
        {
            return _repository.Select<Role>()
                .SingleOrDefault(role => role.Id == roleId);
        }

        public Permission GetPermission(long permissionId)
        {
            return _repository.Select<Permission>()
                .SingleOrDefault(permission => permission.Id == permissionId);
        }

        public IDictionary<long, Permission[]> GetPermissions(params long[] roleIds)
        {
            var rpQuery = _repository.Select<RolePermission>();
            var pQuery = _repository.Select<Permission>();

            var permissions =
                from rolePermission in rpQuery
                from permission in pQuery
                where
                    rolePermission.PermissionId == permission.Id &&
                    roleIds.Contains(rolePermission.RoleId)
                    orderby permission.Id
                select new
                {
                    rolePermission.RoleId,
                    Permission = permission
                };

            return permissions
                .Page(1, 100).Items
                //.ToList()
                .GroupBy(p => p.RoleId)
                .ToDictionary(
                    grp => grp.Key,
                    grp => grp.Select(item => item.Permission).ToArray());
        }

        public IDictionary<long, Role[]> GetRoles(params long[] userIds)
        {
            var urQuery = _repository.Select<UserRole>();
            var rQuery = _repository.Select<Role>();

            var roles =
                from userRole in urQuery
                from role in rQuery
                where
                    userRole.RoleId == role.Id &&
                    userIds.Contains(userRole.UserId)
                select new
                {
                    userRole.UserId,
                    Role = role
                };

            return roles
                .ToList()
                .GroupBy(p => p.UserId)
                .ToDictionary(
                    grp => grp.Key,
                    grp => grp.Select(item => item.Role).ToArray());
        }

        public IPage<Role> SearchRoles(RoleFilter filter, int pageIndex = 1, int pageSize = 1000)
        {
            var roles = _repository.Select<Role>();

            if (!String.IsNullOrEmpty(filter.Name))
            {
                roles = roles.Where(r => r.Name.Contains(filter.Name));
            }

            return roles.Page(pageIndex, pageSize);
        }
    }
}
