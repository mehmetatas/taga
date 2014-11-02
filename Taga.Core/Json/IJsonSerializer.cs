using System;

namespace Taga.Core.Json
{
    public interface IJsonSerializer
    {
        string Serialize(object obj);

        object Deserialize(string json, Type targetType);
    }

    public static class JsonSerializerExtensions
    {
        public static T Deserialize<T>(this IJsonSerializer serializer, string json) where T : class, new()
        {
            return (T)serializer.Deserialize(json, typeof(T));
        }
    }
}
