using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taga.UserApp.Core.Database;
using Taga.UserApp.Core.Model.Database;
using Taga.UserApp.Core.Repository;

namespace Taga.UserApp.Tests.DbTests
{
    public abstract partial class UserAppDbTests
    {
        [TestMethod, TestCategory("UserRepository")]
        public void Should_Create_User()
        {
            User user;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                repo.Save(user);
                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = repo.Get(user.Id);
            }

            Assert.AreEqual("taga", user.Username);
            Assert.AreEqual("1234", user.Password);
        }

        [TestMethod, TestCategory("UserRepository")]
        public void Should_Update_User()
        {
            User user;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = new User
                {
                    Username = "aga",
                    Password = "123"
                };

                repo.Save(user);
                db.Save();
            }

            var id = user.Id;

            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = repo.Get(user.Id);

                user.Username = "taga";
                user.Password = "1234";

                repo.Save(user);
                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = repo.Get(id);
            }

            Assert.AreEqual("taga", user.Username);
            Assert.AreEqual("1234", user.Password);
        }

        [TestMethod, TestCategory("UserRepository")]
        public void Should_Delete_User()
        {
            User user;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                repo.Save(user);
                db.Save();
            }

            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IUserRepository>();

                repo.Delete(new User
                {
                    Id = user.Id
                });

                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = repo.Get(user.Id);
            }

            Assert.IsNull(user);
        }

        [TestMethod, TestCategory("UserRepository")]
        public void Should_Get_User_By_Username()
        {
            User user;
            using (var db = Db.ReadWrite())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                repo.Save(user);
                db.Save();
            }

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = repo.GetByUsername(user.Username);
            }

            Assert.AreEqual("taga", user.Username);
            Assert.AreEqual("1234", user.Password);
        }
    }
}
