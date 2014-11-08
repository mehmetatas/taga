using System;
using System.Collections.Generic;
using System.Reflection;

namespace Taga.SimpLinq.QueryBuilder.Impl
{
    class SelectQuery : ISelectQuery
    {
        public SelectQuery(Type fromType)
        {
            FromType = fromType;
            SelectProperties = new List<PropertyInfo>();
            JoinProperties = new List<IJoin>();
            OrderByProperties = new List<IOrderBy>();
        }

        public Type FromType { get; private set; }
        public List<PropertyInfo> SelectProperties { get; private set; }
        public List<IJoin> JoinProperties { get; private set; }
        public List<IOrderBy> OrderByProperties { get; private set; }
        public Where Where { get; set; }
        public Page Page { get; set; }

        Type ISelectQuery.FromType
        {
            get { return FromType; }
        }

        IReadOnlyCollection<PropertyInfo> ISelectQuery.SelectProperties
        {
            get
            {
                return SelectProperties.AsReadOnly();
            }
        }

        IReadOnlyCollection<IJoin> ISelectQuery.JoinProperties
        {
            get { return JoinProperties.AsReadOnly(); }
        }

        IReadOnlyCollection<IOrderBy> ISelectQuery.OrderByProperties
        {
            get { return OrderByProperties.AsReadOnly(); }
        }

        IWhere ISelectQuery.Where
        {
            get { return Where; }
        }

        IPage ISelectQuery.Page
        {
            get { return Page; }
        }
    }
}