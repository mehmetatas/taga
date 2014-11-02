using System.Configuration;
using System.Linq;
using System.Transactions;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using NHibernate.Driver;
using NHibernate.Linq;
using Oracle.ManagedDataAccess.Client;
using Taga.Core.IoC;
using Taga.Core.Repository;
using Taga.Repository.NH;
using Taga.Repository.NH.SpCallBuilders;
using Taga.UserApp.Core.Model.Database;

namespace Taga.UserApp.Tests.DbTests.NH
{
    [TestClass]
    public class NHOracleTests : UserAppDbTests
    {
        protected override void InitDb()
        {
            var sessionFactory = Fluently.Configure()
                .Database(OracleClientConfiguration.Oracle10
                    .Driver<OracleManagedDataClientDriver>()
                    .ConnectionString(cfg => cfg.FromConnectionStringWithKey("user_app_oracle"))
                    .ShowSql())
                .Mappings(mappingConfig => mappingConfig.FluentMappings.AddFromAssemblyOf<User>())
                .BuildSessionFactory();

            var prov = ServiceProvider.Provider;

            prov.RegisterSingleton<INHSpCallBuilder>(new OracleSpCallBuilder());
            prov.Register<IUnitOfWork, NHUnitOfWork>();
            prov.Register<ITransactionalUnitOfWork, NHUnitOfWork>();
            prov.Register<IRepository, NHRepository>();
            prov.RegisterSingleton(sessionFactory);
        }

        protected override DbSystem DbSystem
        {
            get { return DbSystem.Oracle; }
        }

        [TestMethod]
        public void Test_Transaction_Scope()
        {
            var connStr = ConfigurationManager.ConnectionStrings["user_app_oracle"].ConnectionString;
            using (var conn = new OracleConnection(connStr))
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

        [TestMethod]
        public void Test_Begin_Transaction()
        {
            var connStr = ConfigurationManager.ConnectionStrings["user_app_oracle"].ConnectionString;
            using (var conn = new OracleConnection(connStr))
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
    }
}
