using System.Reflection;

namespace Taga.Core.Rest
{
    public class MethodMapping
    {
        public string MethodRoute { get; set; }
        public MethodInfo Method { get; set; }
        public HttpMethodType HttpMethodType { get; set; }
    }
}