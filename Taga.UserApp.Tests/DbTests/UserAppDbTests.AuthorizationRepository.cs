using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taga.UserApp.Core.Database;
using Taga.UserApp.Core.Model.Database;
using Taga.UserApp.Core.Repository;

namespace Taga.UserApp.Tests.DbTests
{
    public abstract partial class UserAppDbTests
    {
        [TestMethod, TestCategory("AuthorizationRepository")]
        public void Should_Create_Role()
        {
            Role role;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                role = new Role
                {
                    Name = "Test Role"
                };

                repo.Save(role);
                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                role = repo.GetRole(role.Id);
            }

            Assert.AreEqual("Test Role", role.Name);
        }

        [TestMethod, TestCategory("AuthorizationRepository")]
        public void Should_Update_Role()
        {
            Role role;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                role = new Role
                {
                    Name = "Test Rol"
                };

                repo.Save(role);
                db.Save();
            }

            var id = role.Id;

            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                role = repo.GetRole(role.Id);

                role.Name = "Test Role";

                repo.Save(role);
                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                role = repo.GetRole(id);
            }

            Assert.AreEqual("Test Role", role.Name);
        }

        [TestMethod, TestCategory("AuthorizationRepository")]
        public void Should_Create_Permission()
        {
            Permission permission;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                permission = new Permission
                {
                    Name = "Test Permission"
                };

                repo.Save(permission);
                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                permission = repo.GetPermission(permission.Id);
            }

            Assert.AreEqual("Test Permission", permission.Name);
        }

        [TestMethod, TestCategory("AuthorizationRepository")]
        public void Should_Update_Permission()
        {
            Permission permission;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                permission = new Permission
                {
                    Name = "Test Perm"
                };

                repo.Save(permission);
                db.Save();
            }

            var id = permission.Id;

            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                permission = repo.GetPermission(permission.Id);

                permission.Name = "Test Permission";

                repo.Save(permission);
                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                permission = repo.GetPermission(id);
            }

            Assert.AreEqual("Test Permission", permission.Name);
        }

        [TestMethod, TestCategory("AuthorizationRepository")]
        public void Should_Insert_Roles_Of_User()
        {
            User user;

            IList<Role> roles = new List<Role>();

            using (var db = Db.ReadWrite())
            {
                var authRepo = db.GetRepository<IAuthorizationRepository>();
                var userRepo = db.GetRepository<IUserRepository>();

                for (var i = 1; i <= 10; i++)
                {
                    var role = new Role
                    {
                        Name = "Test Role " + i
                    };

                    authRepo.Save(role);

                    if (i % 2 == 0)
                    {
                        roles.Add(role);
                    }
                }

                user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                userRepo.Save(user);

                db.Save();

                authRepo.SaveRolesOfUser(user.Id, roles.ToArray());

                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                roles = repo.GetRolesOfUser(user.Id);
            }

            Assert.AreEqual(5, roles.Count);

            var x = 2;
            foreach (var role in roles)
            {
                Assert.AreEqual("Test Role " + x, role.Name);
                x += 2;
            }
        }

        [TestMethod, TestCategory("AuthorizationRepository")]
        public void Should_Insert_Permissions_Of_Role()
        {
            Role role;

            var permissions = new List<Permission>();

            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                for (var i = 1; i <= 10; i++)
                {
                    var permission = new Permission
                    {
                        Name = "Test Permission " + i
                    };

                    repo.Save(permission);

                    if (i % 2 == 0)
                    {
                        permissions.Add(permission);
                    }
                }

                role = new Role
                {
                    Name = "Test Role"
                };

                repo.Save(role);

                db.Save();

                repo.SavePermissionsOfRole(role.Id, permissions.ToArray());

                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                permissions = repo.GetPermissionsOfRole(role.Id).ToList();
            }

            Assert.AreEqual(5, permissions.Count);

            var x = 2;
            foreach (var permission in permissions)
            {
                Assert.AreEqual("Test Permission " + x, permission.Name);
                x += 2;
            }
        }

        [TestMethod, TestCategory("AuthorizationRepository")]
        public void Should_Update_Roles_Of_User()
        {
            User user;

            var allRoles = new List<Role>();
            var assignedRoles = new List<Role>();

            for (var i = 1; i <= 10; i++)
            {
                var role = new Role
                {
                    Name = "Test Role " + i
                };

                allRoles.Add(role);

                if (i % 2 == 0)
                {
                    assignedRoles.Add(role);
                }
            }

            using (var db = Db.ReadWrite())
            {
                var authRepo = db.GetRepository<IAuthorizationRepository>();
                var userRepo = db.GetRepository<IUserRepository>();

                foreach (var role in allRoles)
                {
                    authRepo.Save(role);
                }

                user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                userRepo.Save(user);

                db.Save();

                authRepo.SaveRolesOfUser(user.Id, assignedRoles.ToArray());

                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                assignedRoles = repo.GetRolesOfUser(user.Id).ToList();
            }

            Assert.AreEqual(5, assignedRoles.Count);

            var x = 2;
            foreach (var role in assignedRoles)
            {
                Assert.AreEqual("Test Role " + x, role.Name);
                x += 2;
            }

            assignedRoles.Clear();

            for (var i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    assignedRoles.Add(allRoles[i]);
                }
            }

            using (var db = Db.ReadWrite())
            {
                var authRepo = db.GetRepository<IAuthorizationRepository>();

                authRepo.SaveRolesOfUser(user.Id, assignedRoles.ToArray());

                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                assignedRoles = repo.GetRolesOfUser(user.Id).ToList();
            }

            Assert.AreEqual(5, assignedRoles.Count);

            x = 1;
            foreach (var role in assignedRoles)
            {
                Assert.AreEqual("Test Role " + x, role.Name);
                x += 2;
            }
        }

        [TestMethod, TestCategory("AuthorizationRepository")]
        public void Should_Update_Permissions_Of_Role()
        {
            Role role;

            var allPermissions = new List<Permission>();
            var assignedPermissions = new List<Permission>();

            for (var i = 1; i <= 10; i++)
            {
                var permission = new Permission
                {
                    Name = "Test Permission " + i
                };

                allPermissions.Add(permission);

                if (i % 2 == 0)
                {
                    assignedPermissions.Add(permission);
                }
            }

            using (var db = Db.ReadWrite())
            {
                var authRepo = db.GetRepository<IAuthorizationRepository>();

                foreach (var permission in allPermissions)
                {
                    authRepo.Save(permission);
                }

                role = new Role
                {
                    Name = "Test Role"
                };

                authRepo.Save(role);

                db.Save();

                authRepo.SavePermissionsOfRole(role.Id, assignedPermissions.ToArray());

                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                assignedPermissions = repo.GetPermissionsOfRole(role.Id).ToList();
            }

            Assert.AreEqual(5, assignedPermissions.Count);

            var x = 2;
            foreach (var permission in assignedPermissions)
            {
                Assert.AreEqual("Test Permission " + x, permission.Name);
                x += 2;
            }

            assignedPermissions.Clear();

            for (var i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                {
                    assignedPermissions.Add(allPermissions[i]);
                }
            }

            using (var db = Db.ReadWrite())
            {
                var authRepo = db.GetRepository<IAuthorizationRepository>();

                authRepo.SavePermissionsOfRole(role.Id, assignedPermissions.ToArray());

                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IAuthorizationRepository>();

                assignedPermissions = repo.GetPermissionsOfRole(role.Id).ToList();
            }

            Assert.AreEqual(5, assignedPermissions.Count);

            x = 1;
            foreach (var permission in assignedPermissions)
            {
                Assert.AreEqual("Test Permission " + x, permission.Name);
                x += 2;
            }
        }
    }
}
