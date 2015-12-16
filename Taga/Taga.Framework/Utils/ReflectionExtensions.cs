using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Taga.Framework.Utils
{
    public static class ReflectionExtensions
    {
        public static PropertyInfo GetPropertyInfo<TClass, TProp>(this Expression<Func<TClass, TProp>> propExp)
        {
            MemberExpression memberExp;

            var unaryExpression = propExp.Body as UnaryExpression;
            if (unaryExpression == null)
            {
                memberExp = (MemberExpression)propExp.Body;
            }
            else
            {
                memberExp = (MemberExpression)unaryExpression.Operand;
            }

            return memberExp.Member as PropertyInfo;
        }
    }
}
