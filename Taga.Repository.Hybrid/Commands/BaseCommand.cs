using System;
using System.Data;
using Taga.Core.IoC;
using Taga.Core.Repository.Mapping;
using Taga.Core.Repository.Hybrid;

namespace Taga.Repository.Hybrid.Commands
{
    abstract class BaseCommand : IHybridUowCommand
    {
        protected readonly object Entity;
        protected readonly Type EntityType;

        protected BaseCommand(object entity)
        {
            Entity = entity;
            EntityType = entity.GetType();
        }

        protected static IMappingProvider MappingProvider
        {
            get
            {
                return ServiceProvider.Provider.GetOrCreate<IMappingProvider>();
            }
        }

        protected static IHybridDbProvider HybridDbProvider
        {
            get
            {
                return ServiceProvider.Provider.GetOrCreate<IHybridDbProvider>();
            }
        }

        public abstract void Execute(IDbCommand cmd);

        protected static string GetParamName(string columnName)
        {
            return String.Format("{0}p_{1}", HybridDbProvider.ParameterPrefix, columnName);
        }
    }
}