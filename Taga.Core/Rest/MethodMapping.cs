using System.Data;
using System.Reflection;

namespace Taga.Core.Rest
{
    public class MethodMapping
    {
        public static bool DefaultIsTransactional = true;
        public static IsolationLevel DefaultTransactionIsolationLevel = IsolationLevel.ReadCommitted;
        
        public MethodMapping()
        {
            IsTransactional = DefaultIsTransactional;
            TransactionIsolationLevel = DefaultTransactionIsolationLevel;
        }

        public MethodInfo Method { get; set; }
        public string MethodRoute { get; set; }
        public HttpMethodType HttpMethodType { get; set; }
        public bool IsTransactional { get; set; }
        public IsolationLevel TransactionIsolationLevel { get; set; }
    }
}