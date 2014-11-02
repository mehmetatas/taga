using System;

namespace Taga.Core.Mapping
{
    public interface IMapper
    {
        IMappingRuleChain<TSource, TTarget> Register<TSource, TTarget>()
            where TSource : class
            where TTarget : class;

        TTarget Map<TTarget>(object source)
            where TTarget : class;
    }

    public interface IMappingRuleChain<out TSource, out TTarget>
        where TSource : class
        where TTarget : class
    {
        IMappingRuleChain<TSource, TTarget> Customize(Action<TSource, TTarget> afterFunction);
    }
}