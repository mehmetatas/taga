using System.Data.Common;
using System.Data.Entity;
using Taga.Repository.EF;

namespace Taga.UserApp.Core.Database.EF
{
    public class UserAppEFQueryProvider : EFHybridQueryProvider
    {
        protected override DbContext CreateDbContext(DbConnection connection)
        {
            return new UserAppContext(connection);
        }
    }
}
