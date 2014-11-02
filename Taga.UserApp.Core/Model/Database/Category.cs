using System;

namespace Taga.UserApp.Core.Model.Database
{
    public class Category
    {
        public virtual long Id { get; set; }
        public virtual long UserId { get; set; }
        public virtual string Title { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreateDate { get; set; }

        public virtual User User { get; set; }
    }
}
