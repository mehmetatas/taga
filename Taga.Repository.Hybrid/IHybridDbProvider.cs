using System;
using System.Data;

namespace Taga.Repository.Hybrid
{
    public interface IHybridDbProvider
    {
        char ParameterPrefix { get; }

        object Insert(Type type, IDbCommand insertCommand, bool selectId);

        IDbConnection CreateConnection();
    }
}