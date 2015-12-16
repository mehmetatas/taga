using Taga.Orm.Dynamix;

namespace Taga.Orm.Meta
{
    public interface IAssociationMeta
    {
        IAssociationLoader Loader { get; }
    }
}