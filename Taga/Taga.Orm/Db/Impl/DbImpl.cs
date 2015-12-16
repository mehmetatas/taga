using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using Taga.Orm.Dynamix.Impl;
using Taga.Orm.Meta;
using Taga.Orm.Sql.Command;

namespace Taga.Orm.Db.Impl
{
    public class DbImpl : IDb, ICommandExecutor
    {
        private IDbTransaction _tran;
        private readonly IDbConnection _conn;
        private readonly IDbMeta _meta;

        protected internal DbImpl(IDbMeta meta)
        {
            _meta = meta;
            _conn = meta.DbProvider.CreateConnection();
            _conn.Open();
        }

        public virtual void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            if (_tran == null)
            {
                _tran = _conn.BeginTransaction(isolationLevel);
            }
        }

        public virtual void Commit()
        {
            _tran?.Commit();
        }

        public virtual void Rollback()
        {
            _tran?.Rollback();
        }

        public virtual void Insert<T>(T entity) where T : class, new()
        {
            var cmd = SimpleCmd<T>().BuildInsertCommand(entity);
            var id = ExecuteScalar(cmd);

            if (id == null)
            {
                return;
            }

            var tableMeta = _meta.GetTable<T>();
            tableMeta.IdColumn.GetterSetter.Set(entity, id);
        }

        public virtual void Update<T>(T entity) where T : class, new()
        {
            var cmd = SimpleCmd<T>().BuildUpdateCommand(entity);
            ExecuteNonQuery(cmd);
        }

        public virtual void Delete<T>(T entity) where T : class, new()
        {
            var cmd = SimpleCmd<T>().BuildDeleteCommand(entity);
            ExecuteNonQuery(cmd);
        }

        public virtual void DeleteMany<T>(Expression<Func<T, bool>> filter) where T : class, new()
        {
            var builder = _meta.DbProvider.CreateDeleteManyCommandBuilder(_meta);
            var cmd = builder.Build(filter);
            ExecuteNonQuery(cmd);
        }

        public virtual T GetById<T>(object id) where T : class, new()
        {
            var cmd = SimpleCmd<T>().BuildGetByIdCommand(id);
            using (var reader = ExecuteReaderInternal(cmd))
            {
                if (!reader.Read())
                {
                    return null;
                }
                var deserializer = PocoDeserializer.GetDefault<T>();
                return deserializer.Deserialize(reader) as T;
            }
        }

        public virtual IQuery<T> Select<T>() where T : class, new()
        {
            return new QueryImpl<T>(_meta, this);
        }

        public virtual IList<T> ExecuteQuery<T>(Command selectCommand) where T : class, new()
        {
            using (var reader = ExecuteReaderInternal(selectCommand))
            {
                var deserializer = PocoDeserializer.GetDefault<T>();

                var list = new List<T>();

                while (reader.Read())
                {
                    var entity = (T)deserializer.Deserialize(reader);
                    list.Add(entity);
                }

                return list;
            }
        }

        public virtual int ExecuteNonQuery(Command cmd)
        {
            using (var dbCmd = CreateCommand(cmd))
            {
                return dbCmd.ExecuteNonQuery();
            }
        }

        public virtual object ExecuteScalar(Command cmd)
        {
            using (var dbCmd = CreateCommand(cmd))
            {
                return dbCmd.ExecuteScalar();
            }
        }

        public virtual void Load<T, TProp>(IList<T> entities, Expression<Func<T, TProp>> propExp, Expression<Func<TProp, object>> includeProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            var colMeta = _meta.GetColumn(propExp);
            colMeta.Loader.Load(entities, this, includeProps);
        }

        public virtual void LoadMany<T, TProp>(IList<T> entities, Expression<Func<T, IList<TProp>>> listExp, Expression<Func<TProp, object>> includeProps = null)
            where T : class, new()
            where TProp : class, new()
        {
            var assoc = _meta.GetAssociation(listExp);
            assoc.Loader.Load(entities, this, includeProps);
        }

        public virtual void Dispose()
        {
            _conn.Dispose();
            _tran?.Dispose();
        }

        IDataReader ICommandExecutor.ExecuteReader(Command cmd)
        {
            return ExecuteReaderInternal(cmd);
        }

        int ICommandExecutor.ExecuteNonQuery(Command cmd)
        {
            return ExecuteNonQuery(cmd);
        }

        object ICommandExecutor.ExecuteScalar(Command cmd)
        {
            return ExecuteScalar(cmd);
        }

        private ISimpleCommandBuilder SimpleCmd<T>() where T : class, new()
        {
            return _meta.GetTable<T>().SimpleCommandBuilder;
        }

        private IDbCommand CreateCommand(Command cmd)
        {
            var dbCmd = _conn.CreateCommand();

            dbCmd.Transaction = _tran;

            dbCmd.CommandText = cmd.CommandText;
            dbCmd.CommandType = cmd.Type;

            foreach (var sqlParameter in cmd.Parameters)
            {
                var param = dbCmd.CreateParameter();

                var paramMeta = sqlParameter.Value.ParameterMeta;

                param.ParameterName = sqlParameter.Key;
                param.Value = sqlParameter.Value.Value ?? DBNull.Value;

                param.DbType = paramMeta.DbType;
                param.Size = paramMeta.Size;
                param.Scale = paramMeta.Scale;
                param.Precision = paramMeta.Precision;

                dbCmd.Parameters.Add(param);
            }
#if DEBUG
            Console.WriteLine(dbCmd.CommandText);
            //foreach (var param in dbCmd.Parameters.Cast<IDbDataParameter>())
            //{
            //    Console.WriteLine("{0}: {1}", param.ParameterName, param.Value);
            //}
#endif
            return dbCmd;
        }

        private IDataReader ExecuteReaderInternal(Command cmd)
        {
            using (var dbCmd = CreateCommand(cmd))
            {
                return dbCmd.ExecuteReader();
            }
        }
    }
}