using System.Reflection;

namespace Taga.Core.Rest
{
    public class MethodMapping
    {
        public MethodInfo Method { get; set; }
        public string MethodRoute { get; set; }
        public HttpMethodType HttpMethodType { get; set; }
    }
}