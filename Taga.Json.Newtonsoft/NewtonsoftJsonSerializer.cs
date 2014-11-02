using Newtonsoft.Json;
using System;
using Taga.Core.Json;

namespace Taga.Json.Newtonsoft
{
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public object Deserialize(string json, Type targetType)
        {
            return JsonConvert.DeserializeObject(json, targetType);
        }
    }
}
