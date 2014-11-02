using System;
using AutoMapper;
using Taga.Core.Mapping;

namespace Taga.Mapping.AutoMapper
{
    class AutoMapperMappingRuleChain<TSource, TTarget> : IMappingRuleChain<TSource, TTarget>
        where TSource : class
        where TTarget : class
    {
        private readonly IMappingExpression<TSource, TTarget> _expression;

        public AutoMapperMappingRuleChain(IMappingExpression<TSource, TTarget> expression)
        {
            _expression = expression;
        }

        public IMappingRuleChain<TSource, TTarget> Customize(Action<TSource, TTarget> customizationFunction)
        {
            _expression.AfterMap(customizationFunction);
            return this;
        }
    }
}