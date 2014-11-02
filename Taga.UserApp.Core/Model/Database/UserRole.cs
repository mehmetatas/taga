
namespace Taga.UserApp.Core.Model.Database
{
    public class UserRole
    {
        public virtual long UserId { get; set; }
        public virtual long RoleId { get; set; }

        public override bool Equals(object obj)
        {
            var that = obj as UserRole;

            if (that == null)
            {
                return false;
            }

            return UserId == that.UserId &&
                   RoleId == that.RoleId;
        }

        public override int GetHashCode()
        {
            return UserId.GetHashCode() + RoleId.GetHashCode();
        }
    }
}
