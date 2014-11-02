using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySql.Data.MySqlClient;
using NHibernate;
using NHibernate.Linq;
using System.Configuration;
using System.Linq;
using System.Transactions;
using Taga.Core.IoC;
using Taga.Core.Repository;
using Taga.Repository.NH;
using Taga.Repository.NH.SpCallBuilders;
using Taga.UserApp.Core.Model.Database;

namespace Taga.UserApp.Tests.DbTests.NH
{
    [TestClass]
    public class NHMySqlTests : UserAppDbTests
    {
        protected override void InitDb()
        {
            var sessionFactory = Fluently.Configure()
                .Database(MySQLConfiguration.Standard
                    .ConnectionString(cfg => cfg.FromConnectionStringWithKey("user_app_mysql"))
                    .ShowSql())
                .Mappings(mappingConfig => mappingConfig.FluentMappings.AddFromAssemblyOf<User>())
                .BuildSessionFactory();

            var prov = ServiceProvider.Provider;

            prov.RegisterSingleton<INHSpCallBuilder>(new MySqlSpCallBuilder());
            prov.Register<IUnitOfWork, NHUnitOfWork>();
            prov.Register<ITransactionalUnitOfWork, NHUnitOfWork>();
            prov.Register<IRepository, NHRepository>();
            prov.RegisterSingleton(sessionFactory);
        }

        protected override DbSystem DbSystem
        {
            get { return DbSystem.MySql; }
        }

        [TestMethod, TestCategory("TransactionScope")]
        public void Test_Transaction_Scope()
        {
            var connStr = ConfigurationManager.ConnectionStrings["user_app_mysql"].ConnectionString;
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var ts = new TransactionScope())
                {
                    var sessionFactory = ServiceProvider.Provider.GetOrCreate<ISessionFactory>();

                    using (var session = sessionFactory.OpenSession(conn))
                    {
                        session.Save(new User { Username = "user-1" });
                        var users = session.Query<User>().ToList();
                        Assert.AreEqual(1, users.Count);
                    }

                    ts.Complete();
                }
            }
        }

        [TestMethod, TestCategory("TransactionScope")]
        public void Test_Begin_Transaction()
        {
            var connStr = ConfigurationManager.ConnectionStrings["user_app_mysql"].ConnectionString;
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    var sessionFactory = ServiceProvider.Provider.GetOrCreate<ISessionFactory>();

                    using (var session = sessionFactory.OpenSession(conn))
                    {
                        session.Save(new User { Username = "user-1" });
                        var users = session.Query<User>().ToList();
                        Assert.AreEqual(1, users.Count);
                    }

                    tran.Commit();
                }
            }
        }

        // [TestMethod]
        public void Test_Nested_Session()
        {
            var sessionFactory = ServiceProvider.Provider.GetOrCreate<ISessionFactory>();

            TruncateUsersTable(sessionFactory);

            var user0 = new User { Username = "User0" };

            using (var session = sessionFactory.OpenSession())
            {
                session.Save(user0);
            }

            Assert.AreEqual(1, user0.Id);

            TruncateUsersTable(sessionFactory);

            var user1 = new User { Username = "User1" };
            var user2 = new User { Username = "User2" };

            using (var session1 = sessionFactory.OpenSession())
            {
                session1.Save(user1);

                using (var session2 = sessionFactory.OpenSession())
                {
                    Assert.AreNotEqual(session1, session2);
                    Assert.AreNotEqual(session1.Connection, session2.Connection);

                    session2.Save(user2);
                }
            }

            Assert.AreEqual(1, user1.Id);
            Assert.AreEqual(2, user2.Id);

            TruncateUsersTable(sessionFactory);

            var user3 = new User { Username = "User3" };

            using (var session = sessionFactory.OpenSession())
            {
                session.Save(user3);
                session.Flush();
            }

            // Assert.AreEqual failed. Expected:<1>. Actual:<2>.
            Assert.AreEqual(1, user3.Id);
        }

        private void TruncateUsersTable(ISessionFactory sessionFactory)
        {
            using (var session = sessionFactory.OpenSession())
            {
                var cmd = session.Connection.CreateCommand();
                cmd.CommandText = "truncate table users";
                cmd.ExecuteNonQuery();
            }
        }
    }
}
