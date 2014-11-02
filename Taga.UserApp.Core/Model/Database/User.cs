using System.Collections.Generic;

namespace Taga.UserApp.Core.Model.Database
{
    public class User
    {
        public virtual long Id { get; set; }
        public virtual string Username { get; set; }
        public virtual string Password { get; set; }

        public virtual List<Role> Roles { get; set; }
        public virtual List<Category> Categories { get; set; }
    }
}
