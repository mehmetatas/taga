using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Taga.Core.Validation
{
    public interface IValidationRuleBuilder<T>
    {
        PropertyValidatorBuilder<T, TProperty> RuleFor<TProperty>(Expression<Func<T, TProperty>> propExpression);
        
        List<IValidator> Build();
    }
}