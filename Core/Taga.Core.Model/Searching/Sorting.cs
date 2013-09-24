using System;
using System.Linq.Expressions;

namespace Taga.Core.Model.Searching
{
    public class Sorting<TEntity> where TEntity : class, IEntity
    {
        public Expression<Func<TEntity, object>> Expression { get; set; }
        public bool Descending { get; set; } 
    }
}
