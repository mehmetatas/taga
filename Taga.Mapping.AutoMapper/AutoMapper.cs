using Taga.Core.Mapping;
using OtoMapper = AutoMapper.Mapper;

namespace Taga.Mapping.AutoMapper
{
    public class AutoMapper : IMapper
    {
        public IMappingRuleChain<TSource, TTarget> Register<TSource, TTarget>()
            where TSource : class
            where TTarget : class
        {
            var expression = OtoMapper.CreateMap<TSource, TTarget>();
            return new AutoMapperMappingRuleChain<TSource, TTarget>(expression);
        }

        public TTarget Map<TTarget>(object source)
            where TTarget : class
        {
            if (source == null)
                return null;
            return OtoMapper.Map<TTarget>(source);
        }
    }
}