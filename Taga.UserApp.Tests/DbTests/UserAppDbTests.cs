using Microsoft.VisualStudio.TestTools.UnitTesting;
using Taga.Core.Repository;

namespace Taga.UserApp.Tests.DbTests
{
    public abstract partial class UserAppDbTests
    {
        protected UserAppDbTests()
        {
            DbTestInitializer.Initialize(DbSystem);
            InitDb();
            TestCleanup();
        }

        protected abstract void InitDb();

        protected abstract DbSystem DbSystem { get; }

        [TestCleanup]
        public void TestCleanup()
        {
            DbTestInitializer.ClearDb();
        }
    }
}
