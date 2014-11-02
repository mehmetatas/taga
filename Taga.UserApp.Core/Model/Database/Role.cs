using System.Collections.Generic;

namespace Taga.UserApp.Core.Model.Database
{
    public class Role
    {
        public virtual long Id { get; set; }
        public virtual string Name { get; set; }

        public virtual List<Permission> Permissions { get; set; }
    }
}
