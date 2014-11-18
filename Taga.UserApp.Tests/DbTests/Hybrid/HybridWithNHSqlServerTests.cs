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
    public class HybridWithNHSqlServerTests : UserAppDbTests
    {
        protected override void InitDb()
        {
            var sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                    .ConnectionString(ConfigurationManager.ConnectionStrings["user_app_sqlserver"].ConnectionString)
                    .ShowSql())
                .Mappings(mappingConfig => mappingConfig.FluentMappings.AddFromAssemblyOf<User>())
                .BuildSessionFactory();

            var prov = ServiceProvider.Provider;
            
            prov.RegisterSingleton<INHSpCallBuilder>(new SqlServerSpCallBuilder());
            prov.RegisterSingleton<IHybridDbProvider>(new HybridSqlServerProvider());
            prov.Register<IHybridAdapter, NHHybridAdapter>();
            prov.Register<IUnitOfWork, HybridUnitOfWork>();
            prov.Register<ITransactionalUnitOfWork, HybridUnitOfWork>();
            prov.Register<IRepository, HybridRepository>();
            prov.RegisterSingleton(sessionFactory);
        }

        protected override DbSystem DbSystem
        {
            get { return DbSystem.SqlServer; }
        }
    }
}
