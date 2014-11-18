using System;
using System.Data;

namespace Taga.Core.Repository.Hybrid
{
    public interface IHybridDbProvider
    {
        char ParameterPrefix { get; }

        object Insert(Type type, IDbCommand insertCommand, bool selectId);
    }
}