using Taga.UserApp.Core.Database.EF;

namespace Taga.UserApp.Tests.DbTests.EF
{
    public class TestUserAppContext : UserAppContext
    {
        public TestUserAppContext()
            : base("user_app_sqlserver")
        {

        }
    }
}
