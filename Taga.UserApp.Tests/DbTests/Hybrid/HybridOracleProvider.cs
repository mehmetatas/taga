using Oracle.ManagedDataAccess.Client;
using System;
using System.Configuration;
using System.Data;
using Taga.Core.Repository.Hybrid;

namespace Taga.UserApp.Tests.DbTests.Hybrid
{
    class HybridOracleProvider : IHybridDbProvider
    {
        public char ParameterPrefix
        {
            get { return ':'; }
        }

        public object Insert(Type type, IDbCommand cmd, bool selectId)
        {
            if (!selectId)
            {
                cmd.ExecuteNonQuery();
                return null;
            }

            cmd.CommandText += " returning id into :scope_identity";
            var param = cmd.CreateParameter();

            param.ParameterName = "scope_identity";
            param.DbType = DbType.Int64;
            param.Direction = ParameterDirection.ReturnValue;

            cmd.Parameters.Add(param);

            cmd.ExecuteNonQuery();

            return param.Value;
        }

        public IDbConnection CreateConnection()
        {
            return new OracleConnection(ConfigurationManager.ConnectionStrings["user_app_oracle"].ConnectionString);
        }
    }
}
