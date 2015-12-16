using System.Data;

namespace Taga.Orm.Dynamix
{
    public interface IPocoDeserializer
    {
        object Deserialize(IDataReader reader);
    }
}