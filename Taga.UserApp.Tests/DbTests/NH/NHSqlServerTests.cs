using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Linq;
using System.Configuration;
using System.Data.SqlClient;
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
    public class NHSqlServerTests : UserAppDbTests
    {
        protected override void InitDb()
        {
            var sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                    .ConnectionString(cfg => cfg.FromConnectionStringWithKey("user_app_sqlserver"))
                    .ShowSql())
                .Mappings(mappingConfig => mappingConfig.FluentMappings.AddFromAssemblyOf<User>())
                .BuildSessionFactory();

            var prov = ServiceProvider.Provider;

            prov.RegisterSingleton<INHSpCallBuilder>(new SqlServerSpCallBuilder());
            prov.Register<IUnitOfWork, NHUnitOfWork>();
            prov.Register<ITransactionalUnitOfWork, NHUnitOfWork>();
            prov.Register<IRepository, NHRepository>();
            prov.RegisterSingleton(sessionFactory);
        }

        protected override DbSystem DbSystem
        {
            get { return DbSystem.SqlServer; }
        }

        [TestMethod, TestCategory("TransactionScope")]
        public void Test_Transaction_Scope()
        {
            var connStr = ConfigurationManager.ConnectionStrings["user_app_sqlserver"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
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
            var connStr = ConfigurationManager.ConnectionStrings["user_app_sqlserver"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
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

        [TestMethod, TestCategory("TransactionScope")]
        public void Test_Begin_Transaction_Begin_Transaction()
        {
            var connStr = ConfigurationManager.ConnectionStrings["user_app_sqlserver"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var tran1 = conn.BeginTransaction())
                {
                    var sessionFactory = ServiceProvider.Provider.GetOrCreate<ISessionFactory>();

                    using (var session = sessionFactory.OpenSession(conn))
                    {
                        using (var tran2 = session.BeginTransaction())
                        {
                            session.Save(new User { Username = "user-1" });
                            var users = session.Query<User>().ToList();
                            Assert.AreEqual(1, users.Count);
                            tran2.Commit();
                        }
                    }

                    tran1.Commit();
                }
            }
        }

        [TestMethod, TestCategory("TransactionScope")]
        public void Test_Begin_Transaction_Transaction_Scope()
        {
            var connStr = ConfigurationManager.ConnectionStrings["user_app_sqlserver"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();

                using (var tran = conn.BeginTransaction())
                {
                    var sessionFactory = ServiceProvider.Provider.GetOrCreate<ISessionFactory>();
                    using (var session = sessionFactory.OpenSession(conn))
                    {
                        using (var ts = new TransactionScope())
                        {
                            session.Save(new User { Username = "user-1" });
                            var users = session.Query<User>().ToList();
                            Assert.AreEqual(1, users.Count);
                            ts.Complete();
                        }
                    }

                    tran.Commit();
                }
            }
        }
    }
}
