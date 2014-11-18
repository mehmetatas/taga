using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Driver;
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
    public class HybridWithNHOracleTests : UserAppDbTests
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
            prov.RegisterSingleton<IHybridDbProvider>(new HybridOracleProvider());
            prov.Register<IHybridAdapter, NHHybridAdapter>();
            prov.Register<IUnitOfWork, HybridUnitOfWork>();
            prov.Register<ITransactionalUnitOfWork, HybridUnitOfWork>();
            prov.Register<IRepository, HybridRepository>();
            prov.RegisterSingleton(sessionFactory);
        }

        protected override DbSystem DbSystem
        {
            get { return DbSystem.Oracle; }
        }
    }
}
