
namespace Taga.UserApp.Core.Model.Database
{
    public class RolePermission
    {
        public virtual long RoleId { get; set; }
        public virtual long PermissionId { get; set; }

        public override bool Equals(object obj)
        {
            var that = obj as RolePermission;

            if (that == null)
            {
                return false;
            }

            return RoleId == that.RoleId &&
                   PermissionId == that.PermissionId;
        }

        public override int GetHashCode()
        {
            return RoleId.GetHashCode() + PermissionId.GetHashCode();
        }
    }
}
