using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taga.UserApp.Core.Database;
using Taga.UserApp.Core.Model.Database;
using Taga.UserApp.Core.Repository;

namespace Taga.UserApp.Tests.DbTests
{
    public abstract partial class UserAppDbTests
    {
        [TestMethod, TestCategory("Transaction")]
        public void Should_Have_Id_After_Db_Save()
        {
            using (var db = Db.Transactional())
            {
                db.BeginTransaction();

                var repo = db.GetRepository<IUserRepository>();

                var user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                repo.Save(user);

                // Assert.AreEqual(0L, user.Id);

                db.Save(true);

                Assert.IsTrue(user.Id > 0);
            }
        }

        [TestMethod, TestCategory("Transaction")]
        public void Should_Rollback_If_True_Is_Not_Passed_To_Save()
        {
            User user;

            using (var db = Db.Transactional())
            {
                db.BeginTransaction();

                var repo = db.GetRepository<IUserRepository>();

                user = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                repo.Save(user);

                db.Save();
            }

            Assert.IsTrue(user.Id > 0);

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IUserRepository>();

                user = repo.Get(user.Id);
            }

            Assert.IsNull(user);
        }

        [TestMethod, TestCategory("Transaction")]
        public void Should_Rollback_Both()
        {
            User user1;
            User user2;

            using (var db = Db.Transactional())
            {
                db.BeginTransaction();

                var repo = db.GetRepository<IUserRepository>();

                user1 = new User
                {
                    Username = "taga",
                    Password = "1234"
                };
                repo.Save(user1);
                db.Save();

                user2 = new User
                {
                    Username = "taga-2",
                    Password = "1234-2"
                };
                repo.Save(user2);
                db.Save();
            }

            Assert.IsTrue(user1.Id > 0);
            Assert.IsTrue(user2.Id > 0);

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IUserRepository>();

                user1 = repo.Get(user1.Id);
                user2 = repo.Get(user2.Id);
            }

            Assert.IsNull(user1);
            Assert.IsNull(user2);
        }

        [TestMethod, TestCategory("Transaction")]
        public void Should_Insert_Both()
        {
            User user1;
            User user2;

            using (var db = Db.Transactional())
            {
                db.BeginTransaction();

                var repo = db.GetRepository<IUserRepository>();

                user1 = new User
                {
                    Username = "taga",
                    Password = "1234"
                };

                repo.Save(user1);

                db.Save(true);

                user2 = new User
                {
                    Username = "taga-2",
                    Password = "1234-2"
                };

                repo.Save(user2);

                db.Save();
            }

            Assert.IsTrue(user1.Id > 0);
            Assert.IsTrue(user2.Id > 0);

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IUserRepository>();

                user1 = repo.Get(user1.Id);
                user2 = repo.Get(user2.Id);
            }

            Assert.IsNotNull(user1);
            Assert.IsNotNull(user2);
        }

        [TestMethod, TestCategory("Transaction")]
        [ExpectedException(typeof(NotSupportedException))]
        public void Should_Insert_First()
        {
            User user1;
            User user2;

            using (var db = Db.Transactional())
            {
                db.BeginTransaction();

                var repo = db.GetRepository<IUserRepository>();

                user1 = new User
                {
                    Username = "taga",
                    Password = "1234"
                };
                repo.Save(user1);

                using (var db2 = Db.Transactional())
                {
                    db2.BeginTransaction();

                    var repo2 = db2.GetRepository<IUserRepository>();

                    user2 = new User
                    {
                        Username = "taga-2",
                        Password = "1234-2"
                    };
                    repo2.Save(user2);
                    db2.Save();
                }

                db.Save(true);
            }

            Assert.IsTrue(user1.Id > 0);
            Assert.IsTrue(user2.Id > 0);

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IUserRepository>();

                user1 = repo.Get(user1.Id);
                user2 = repo.Get(user2.Id);
            }

            Assert.IsNotNull(user1);
            Assert.IsNull(user2);
        }

        [TestMethod, TestCategory("Transaction")]
        [ExpectedException(typeof(NotSupportedException))]
        public void Should_Insert_Second()
        {
            User user1;
            User user2;

            using (var db = Db.Transactional())
            {
                db.BeginTransaction();

                var repo = db.GetRepository<IUserRepository>();

                user1 = new User
                {
                    Username = "taga",
                    Password = "1234"
                };
                repo.Save(user1);

                using (var db2 = Db.Transactional())
                {
                    db2.BeginTransaction();

                    var repo2 = db2.GetRepository<IUserRepository>();

                    user2 = new User
                    {
                        Username = "taga-2",
                        Password = "1234-2"
                    };
                    repo2.Save(user2);
                    db2.Save(true);
                }

                db.Save();
            }

            Assert.IsTrue(user1.Id > 0);
            Assert.IsTrue(user2.Id > 0);

            using (var db = Db.Readonly())
            {
                var repo = db.GetRepository<IUserRepository>();

                user1 = repo.Get(user1.Id);
                user2 = repo.Get(user2.Id);
            }

            Assert.IsNull(user1);
            Assert.IsNotNull(user2);
        }
    }
}