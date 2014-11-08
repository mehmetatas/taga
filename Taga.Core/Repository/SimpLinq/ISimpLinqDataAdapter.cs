using System;
using System.Collections.Generic;
using Taga.Core.Repository.Command;

namespace Taga.Core.Repository.SimpLinq
{
    public interface ISimpLinqDataAdapter
    {
        IList<T> List<T>(ICommand cmd)
            where T : class, new();

        IPage<T> Page<T>(ICommand cmd, int pageIndex, int pageSize)
            where T : class, new();

        IList<Tuple<T1, T2>> List<T1, T2>(ICommand cmd)
            where T1 : class, new()
            where T2 : class, new();

        IPage<Tuple<T1, T2>> Page<T1, T2>(ICommand cmd, int pageIndex, int pageSize)
            where T1 : class, new()
            where T2 : class, new();

        IList<Tuple<T1, T2, T3>> List<T1, T2, T3>(ICommand cmd)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new();

        IPage<Tuple<T1, T2, T3>> Page<T1, T2, T3>(ICommand cmd, int pageIndex, int pageSize)
            where T1 : class, new()
            where T2 : class, new()
            where T3 : class, new();
    }
}
