using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using Taga.Core.IoC;
using Taga.Core.Repository;
using Taga.Repository.EF;

namespace Taga.UserApp.Tests.DbTests.EF
{
    [TestClass]
    public class EFSqlServerTests : UserAppDbTests
    {
        protected override void InitDb()
        {
            var prov = ServiceProvider.Provider;

            prov.RegisterTransient<DbContext, TestUserAppContext>();
            prov.RegisterTransient<IUnitOfWork, EFUnitOfWork>();
            prov.RegisterTransient<ITransactionalUnitOfWork, EFUnitOfWork>();
            prov.RegisterTransient<IRepository, EFRepository>();
        }

        protected override DbSystem DbSystem
        {
            get { return DbSystem.SqlServer; }
        }
    }
}
