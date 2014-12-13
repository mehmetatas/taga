using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using Taga.Core.IoC;
using Taga.Core.Repository;
using Taga.Core.Repository.Hybrid;
using Taga.Repository.EF;
using Taga.Repository.Hybrid;
using Taga.UserApp.Core.Database.EF;

namespace Taga.UserApp.Tests.DbTests.Hybrid
{
    [TestClass]
    public class HybridWithEFSqlServerTests : UserAppDbTests
    {
        protected override void InitDb()
        {
            var prov = ServiceProvider.Provider;

            prov.RegisterSingleton<IHybridDbProvider>(new HybridSqlServerProvider());
            prov.RegisterTransient<IHybridAdapter, EFHybridAdapter>();
            prov.RegisterTransient<DbContext, UserAppContext>();
            prov.RegisterTransient<IUnitOfWork, HybridUnitOfWork>();
            prov.RegisterTransient<ITransactionalUnitOfWork, HybridUnitOfWork>();
            prov.RegisterTransient<IRepository, HybridRepository>();
        }

        protected override DbSystem DbSystem
        {
            get { return DbSystem.SqlServer; }
        }
    }
}
