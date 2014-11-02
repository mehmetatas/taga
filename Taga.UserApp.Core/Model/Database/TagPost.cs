
namespace Taga.UserApp.Core.Model.Database
{
    public class TagPost
    {
        public virtual long TagId { get; set; }
        public virtual long PostId { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as TagPost;
            return other != null && other.TagId == TagId && other.PostId == PostId;
        }

        public override int GetHashCode()
        {
            return TagId.GetHashCode() + PostId.GetHashCode();
        }
    }
}
