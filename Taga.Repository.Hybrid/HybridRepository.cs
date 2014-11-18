using System.Collections.Generic;
using System.Linq;
using Taga.Core.Repository;
using Taga.Core.Repository.Command;

namespace Taga.Repository.Hybrid
{
    public class HybridRepository : IRepository
    {
        private readonly HybridUnitOfWork _uow;

        private ICommandBuilder _commandBuilder;
        private ICommandBuilder CommandBuilder
        {
            get
            {
                if (_commandBuilder == null)
                {
                    _commandBuilder = Core.Repository.Command.CommandBuilder.CreateBuilder();
                }

                return _commandBuilder;
            }
        }
        
        public HybridRepository()
        {
            _uow = HybridUnitOfWork.Current;
        }

        public void Insert<T>(T entity) where T : class, new()
        {
            _uow.Insert(entity);
        }

        public void Update<T>(T entity) where T : class, new()
        {
            _uow.Update(entity);
        }

        public void Delete<T>(T entity) where T : class, new()
        {
            _uow.Delete(entity);
        }

        public void Flush()
        {
            _uow.Save();
        }

        public IQueryable<T> Select<T>() where T : class, new()
        {
            return _uow.Adapter.Select<T>();
        }

        public IList<T> Query<T>(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false) where T : class, new()
        {
            return _uow.Adapter.Query<T>(spNameOrSql, args, rawSql);
        }

        public void NonQuery(string spNameOrSql, IDictionary<string, object> args = null, bool rawSql = false)
        {
            var cmd = CommandBuilder.BuildCommand(spNameOrSql, args, rawSql);
            _uow.NonQuery(cmd);
        }
    }
}
