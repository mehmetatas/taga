using System.Data.Common;
using Taga.Repository.EF;

namespace Taga.UserApp.Core.Database.EF
{
    public class UserAppContext : TagaDbContext
    {
        static UserAppContext()
        {
            System.Data.Entity.Database.SetInitializer<UserAppContext>(null);
        }

        protected UserAppContext(string connectionStringName)
            : base(connectionStringName)
        {
        }

        public UserAppContext()
            : base("user_app_sqlserver")
        {
        }

        public UserAppContext(DbConnection connection)
            : base(connection)
        {
        }
    }
}
