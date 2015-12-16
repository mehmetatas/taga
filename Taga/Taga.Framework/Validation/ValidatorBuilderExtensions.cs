using System;
using Taga.Framework.Exceptions;
using Taga.Framework.Validation.ValidationRules;

namespace Taga.Framework.Validation
{
    public static class ValidatorBuilderExtensions
    {
        public static IPropertyValidatorBuilder<T, string> Email<T>(this IPropertyValidatorBuilder<T, string> ruleBuilder, Error error)
        {
            return ruleBuilder.AddRule(new EmailRule(), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> Range<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error,
            TProperty min, TProperty max) where TProperty : IComparable<TProperty>
        {
            return ruleBuilder.AddRule(new RangeRule<TProperty>(min, max), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> Null<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error)
            where TProperty : class
        {
            return ruleBuilder.AddRule(new NullRule(), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> NotNull<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error)
            where TProperty : class
        {
            return ruleBuilder.AddRule(new NotNullRule(), error);
        }

        public static IPropertyValidatorBuilder<T, string> Length<T>(this IPropertyValidatorBuilder<T, string> ruleBuilder, Error error,
            int min, int max)
        {
            return ruleBuilder.AddRule(new StringLengthRule(min, max), error);
        }

        public static IPropertyValidatorBuilder<T, string> NotEmpty<T>(this IPropertyValidatorBuilder<T, string> ruleBuilder, Error error)
        {
            return ruleBuilder.AddRule(new StringNotEmptyRule(), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> Equals<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error,
            TProperty expected)
        {
            return ruleBuilder.AddRule(new EqualsRule(expected), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> NotEquals<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error,
            TProperty notExpected)
        {
            return ruleBuilder.AddRule(new NotEqualsRule(notExpected), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> GreaterThan<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error,
            TProperty min) where TProperty : IComparable<TProperty>
        {
            return ruleBuilder.AddRule(new GreaterThanRule<TProperty>(min), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> GreaterThanOrEquals<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error,
            TProperty min) where TProperty : IComparable<TProperty>
        {
            return ruleBuilder.AddRule(new GreaterThanOrEqualsRule<TProperty>(min), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> LessThan<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error,
            TProperty max) where TProperty : IComparable<TProperty>
        {
            return ruleBuilder.AddRule(new LessThanRule<TProperty>(max), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> LessThanOrEquals<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error,
            TProperty max) where TProperty : IComparable<TProperty>
        {
            return ruleBuilder.AddRule(new LessThanOrEqualsRule<TProperty>(max), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> EqualsComparison<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error,
            TProperty expected) where TProperty : IComparable<TProperty>
        {
            return ruleBuilder.AddRule(new EqualsRule<TProperty>(expected), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> NotEqualsComparison<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error,
            TProperty notExpected) where TProperty : IComparable<TProperty>
        {
            return ruleBuilder.AddRule(new NotEqualsRule<TProperty>(notExpected), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> In<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error,
            params TProperty[] values) where TProperty : IComparable<TProperty>
        {
            return ruleBuilder.AddRule(new InRule<TProperty>(values), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> NotIn<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error,
            params TProperty[] values) where TProperty : IComparable<TProperty>
        {
            return ruleBuilder.AddRule(new NotInRule<TProperty>(values), error);
        }

        public static IPropertyValidatorBuilder<T, string> Guid<T>(this IPropertyValidatorBuilder<T, string> ruleBuilder, Error error)
        {
            return ruleBuilder.AddRule(new GuidRule(), error);
        }

        public static IPropertyValidatorBuilder<T, TProperty> Custom<T, TProperty>(this IPropertyValidatorBuilder<T, TProperty> ruleBuilder, Error error,
            Func<T, bool> customValidationFunction)
            where TProperty : class
        {
            return ruleBuilder.AddRule(new CustomRule<T>(customValidationFunction), error);
        }

        public static IPropertyValidatorBuilder<T, string> Regex<T>(this IPropertyValidatorBuilder<T, string> ruleBuilder, Error error,
            string pattern)
        {
            return ruleBuilder.AddRule(new RegexRule(pattern), error);
        }

        public static IPropertyValidatorBuilder<T, string> Charset<T>(this IPropertyValidatorBuilder<T, string> ruleBuilder, Error error,
            string charset, bool caseSensitive, string culture)
        {
            return ruleBuilder.AddRule(new CharsetRule(charset, caseSensitive, culture), error);
        }
    }
}