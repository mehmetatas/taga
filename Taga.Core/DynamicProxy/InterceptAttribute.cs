using System;

namespace Taga.Core.DynamicProxy
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Class)]
    public class InterceptAttribute : Attribute
    {
    }
}