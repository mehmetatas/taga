using System.Data;

namespace Taga.Core.ORM
{
    public interface IPocoMapper
    {
        object Map(IDataReader reader);
    }

    public static class PocoMapperExtensions
    {
        public static T Map<T>(this IPocoMapper mapper, IDataReader reader) where T : class, new()
        {
            return (T)mapper.Map(reader);
        }
    }
}
