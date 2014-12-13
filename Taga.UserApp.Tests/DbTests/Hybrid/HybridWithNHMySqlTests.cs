using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using Taga.Core.IoC;
using Taga.Core.Repository;
using Taga.Core.Repository.Hybrid;
using Taga.Repository.Hybrid;
using Taga.Repository.NH;
using Taga.Repository.NH.SpCallBuilders;
using Taga.UserApp.Core.Model.Database;

namespace Taga.UserApp.Tests.DbTests.Hybrid
{
    [TestClass]
    public class HybridWithNHMySqlTests : UserAppDbTests
    {
        protected override void InitDb()
        {
            var sessionFactory = Fluently.Configure()
                .Database(MySQLConfiguration.Standard
                    .ConnectionString(ConfigurationManager.ConnectionStrings["user_app_mysql"].ConnectionString)
                    .ShowSql())
                .Mappings(mappingConfig => mappingConfig.FluentMappings.AddFromAssemblyOf<User>())
                .BuildSessionFactory();

            var prov = ServiceProvider.Provider;

            prov.RegisterSingleton<INHSpCallBuilder>(new MySqlSpCallBuilder());
            prov.RegisterSingleton<IHybridDbProvider>(new HybridMySqlProvider());
            prov.RegisterTransient<IHybridAdapter, NHHybridAdapter>();
            prov.RegisterTransient<IUnitOfWork, HybridUnitOfWork>();
            prov.RegisterTransient<ITransactionalUnitOfWork, HybridUnitOfWork>();
            prov.RegisterTransient<IRepository, HybridRepository>();
            prov.RegisterSingleton(sessionFactory);
        }

        protected override DbSystem DbSystem
        {
            get { return DbSystem.MySql; }
        }
    }
}
