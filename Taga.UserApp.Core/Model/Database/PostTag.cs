
namespace Taga.UserApp.Core.Model.Database
{
    public class PostTag
    {
        public virtual long PostId { get; set; }
        public virtual long TagId { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as PostTag;
            return other != null && other.PostId == PostId && other.TagId == TagId;
        }

        public override int GetHashCode()
        {
            return PostId.GetHashCode() + TagId.GetHashCode();
        }
    }
}
