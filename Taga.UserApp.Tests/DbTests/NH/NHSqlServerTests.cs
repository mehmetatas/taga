using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
            prov.RegisterTransient<IUnitOfWork, NHUnitOfWork>();
            prov.RegisterTransient<ITransactionalUnitOfWork, NHUnitOfWork>();
            prov.RegisterTransient<IRepository, NHRepository>();
            prov.RegisterSingleton(sessionFactory);
        }

        protected override DbSystem DbSystem
        {
            get { return DbSystem.SqlServer; }
        }
    }
}
