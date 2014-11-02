using FluentNHibernate.Mapping;
using Taga.Repository.NH;
using Taga.UserApp.Core.Model.Database;

namespace Taga.UserApp.Core.Database.NH
{
    #region Tables

    //public class UserMap : ClassMap<User>
    //{
    //    public UserMap()
    //    {
    //        Table("users");
    //        Id(u => u.Id).Column("id").GeneratedBy.Identity();
    //        Map(u => u.Username).Column("username");
    //        Map(u => u.Password).Column("password");
    //    }
    //}

    public class UserMap : TagaClassMap<User>
    {
    }

    public class RoleMap : TagaClassMap<Role>
    {
    }

    public class PermissionMap : TagaClassMap<Permission>
    {
    }

    public class UserRoleMap : TagaClassMap<UserRole>
    {
    }

    public class RolePermissionMap : TagaClassMap<RolePermission>
    {
    }

    public class CategoryMap : TagaClassMap<Category>
    {
    }

    public class PostMap : TagaClassMap<Post>
    {
    }

    public class TagMap : TagaClassMap<Tag>
    {
    }

    public class PostTagMap : TagaClassMap<PostTag>
    {
    }

    public class TagPostMap : TagaClassMap<TagPost>
    {
    }

    #endregion
}