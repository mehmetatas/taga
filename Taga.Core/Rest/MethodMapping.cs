using System.Reflection;

namespace Taga.Core.Rest
{
    public class MethodMapping
    {
        public static bool DefaultRollbackOnError = true;

        public MethodMapping()
        {
            RollbackOnError = DefaultRollbackOnError;
        }

        public bool RollbackOnError { get; set; }
        public string MethodRoute { get; set; }
        public MethodInfo Method { get; set; }
        public HttpMethodType HttpMethodType { get; set; }
    }
}