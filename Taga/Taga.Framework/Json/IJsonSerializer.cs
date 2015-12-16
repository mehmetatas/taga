using System;

namespace Taga.Framework.Json
{
    /// <summary>
    /// Json serializer interface
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        /// Serializes the given object to a json string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        string Serialize(object obj);

        /// <summary>
        /// Deserializes the json string to provided target type instance
        /// </summary>
        /// <param name="json"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        object Deserialize(string json, Type targetType);
    }

    /// <summary>
    /// Json serializer extensions
    /// </summary>
    public static class JsonSerializerExtensions
    {
        /// <summary>
        /// Deserializes and casts the json string to provided target type instance 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializer"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this IJsonSerializer serializer, string json) where T : class
        {
            return (T)serializer.Deserialize(json, typeof(T));
        }
    }
}
