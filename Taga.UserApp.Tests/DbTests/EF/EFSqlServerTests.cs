using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using Taga.Core.IoC;
using Taga.Core.Repository;
using Taga.Repository.EF;
using Taga.UserApp.Core.Database.EF;
using Taga.UserApp.Core.Model.Database;

namespace Taga.UserApp.Tests.DbTests.EF
{
    [TestClass]
    public class EFSqlServerTests : UserAppDbTests
    {
        protected override void InitDb()
        {
            var prov = ServiceProvider.Provider;

            prov.Register<DbContext, TestUserAppContext>();
            prov.Register<IUnitOfWork, EFUnitOfWork>();
            prov.Register<ITransactionalUnitOfWork, EFUnitOfWork>();
            prov.Register<IRepository, EFRepository>();
        }

        protected override DbSystem DbSystem
        {
            get { return DbSystem.SqlServer; }
        }

        [TestMethod]
        public void Test_Transaction_Scope()
        {
            var connStr = ConfigurationManager.ConnectionStrings["user_app_sqlserver"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var ts = new TransactionScope())
                {
                    using (var ctx = new UserAppContext(conn))
                    {
                        var users = ctx.Set<User>();
                        users.Add(new User { Username = "user-1" });
                        ctx.SaveChanges();
                        var userList = users.ToList();
                        Assert.AreEqual(1, userList.Count);
                    }
                    ts.Complete();
                }
            }
        }

        [TestMethod]
        public void Test_Begin_Transaction()
        {
            var connStr = ConfigurationManager.ConnectionStrings["user_app_sqlserver"].ConnectionString;
            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                using (var tran = conn.BeginTransaction())
                {
                    using (var ctx = new UserAppContext(conn))
                    {
                        var users = ctx.Set<User>();
                        users.Add(new User { Username = "user-1" });
                        ctx.SaveChanges();
                        var userList = users.ToList();
                        Assert.AreEqual(1, userList.Count);
                    }
                    tran.Commit();
                }
            }
        }
    }
}
