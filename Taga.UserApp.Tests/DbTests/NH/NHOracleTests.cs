using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate.Driver;
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
            prov.RegisterTransient<IUnitOfWork, NHUnitOfWork>();
            prov.RegisterTransient<ITransactionalUnitOfWork, NHUnitOfWork>();
            prov.RegisterTransient<IRepository, NHRepository>();
            prov.RegisterSingleton(sessionFactory);
        }

        protected override DbSystem DbSystem
        {
            get { return DbSystem.Oracle; }
        }
    }
}
