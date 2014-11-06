using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Taga.SimpLinq.QueryBuilder.Impl
{
    class SelectQuery : ISelectQuery
    {
        public SelectQuery(Type fromType)
        {
            FromType = fromType;
            SelectProperties = new List<PropertyInfo>();
            LeftJoinProperties = new List<IJoin>();
            InnerJoinProperties = new List<IJoin>();
            OrderByProperties = new List<IOrderBy>();
        }

        public Type FromType { get; private set; }
        public List<PropertyInfo> SelectProperties { get; private set; }
        public List<IJoin> LeftJoinProperties { get; private set; }
        public List<IJoin> InnerJoinProperties { get; private set; }
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

        IReadOnlyCollection<IJoin> ISelectQuery.LeftJoinProperties
        {
            get { return LeftJoinProperties.AsReadOnly(); }
        }

        IReadOnlyCollection<IJoin> ISelectQuery.InnerJoinProperties
        {
            get { return InnerJoinProperties.AsReadOnly(); }
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

        public string ToSql()
        {
            var sql = new StringBuilder();

            sql.Append("SELECT")
                .AppendLine()
                .Append("    ")
                .Append(String.Join(String.Format(",{0}    ", Environment.NewLine),
                    SelectProperties.Select(pi => String.Format("{0}.{1} AS {0}_{1}", pi.DeclaringType.Name, pi.Name))))
                .AppendLine()
                .Append("FROM ")
                .Append(FromType.Name);

            if (InnerJoinProperties.Any())
            {
                sql.AppendLine()
                    .Append(String.Join(Environment.NewLine,
                        InnerJoinProperties.Select(pi => String.Format("    INNER JOIN {0} ON {0}.{1} = {2}.{3}",
                            pi.RightProperty.DeclaringType.Name,
                            pi.RightProperty.Name,
                            pi.LeftProperty.DeclaringType.Name,
                            pi.LeftProperty.Name))));
            }

            if (LeftJoinProperties.Any())
            {
                sql.AppendLine()
                    .Append(String.Join(Environment.NewLine,
                        LeftJoinProperties.Select(pi => String.Format("    LEFT JOIN {0} ON {0}.{1} = {2}.{3}",
                            pi.RightProperty.DeclaringType.Name,
                            pi.RightProperty.Name,
                            pi.LeftProperty.DeclaringType.Name,
                            pi.LeftProperty.Name))));
            }

            if (Where != null)
            {
                sql.AppendLine()
                    .Append("WHERE")
                    .AppendLine()
                    .Append("    ")
                    .Append(Where);
            }

            if (OrderByProperties.Any())
            {
                sql.AppendLine()
                    .Append("ORDER BY")
                    .AppendLine()
                    .Append("    ")
                    .Append(String.Join(",", OrderByProperties.Select(ob => String.Format("{0}.{1} {2}",
                        ob.OrderProperty.DeclaringType.Name,
                        ob.OrderProperty.Name,
                        ob.Descending ? "DESC" : "ASC"))));
            }

            if (Page != null)
            {
                sql.AppendLine()
                    .AppendFormat("LIMIT {0} OFFSET {1}", Page.PageSize, (Page.PageIndex - 1) * Page.PageSize);
            }

            return sql.ToString();
        }
    }
}