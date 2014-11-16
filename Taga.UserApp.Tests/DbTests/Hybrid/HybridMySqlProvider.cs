using MySql.Data.MySqlClient;
using System;
using System.Configuration;
using System.Data;
using Taga.Core.Repository.Hybrid;

namespace Taga.UserApp.Tests.DbTests.Hybrid
{
    public class HybridMySqlProvider : IHybridDbProvider
    {
        public char ParameterPrefix
        {
            get { return '?'; }
        }

        public object Insert(Type type, IDbCommand cmd, bool selectId)
        {
            if (selectId)
            {
                cmd.CommandText += "; SELECT LAST_INSERT_ID();";   
            }
            return cmd.ExecuteScalar();
        }

        public IDbConnection CreateConnection()
        {
            return new MySqlConnection(ConfigurationManager.ConnectionStrings["user_app_mysql"].ConnectionString);
        }
    }
}
