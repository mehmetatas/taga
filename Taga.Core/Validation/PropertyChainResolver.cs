using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Taga.Core.Validation
{
    public class PropertyChainResolver
    {
        private readonly Expression _exp;
        public PropertyChainResolver(Expression exp)
        {
            _exp = exp;
        }

        public PropertyInfo[] Resolve()
        {
            var visitor = new PropVisitor();
            visitor.Visit(_exp);
            return visitor.PropInfos.ToArray();
        }

        private class PropVisitor : ExpressionVisitor
        {
            internal readonly Stack<PropertyInfo> PropInfos = new Stack<PropertyInfo>();

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member is PropertyInfo)
                {
                    PropInfos.Push(node.Member as PropertyInfo);
                }
                return base.VisitMember(node);
            }
        }
    }
}
