using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Taga.Orm.Dynamix.Impl
{
    public static class GetterSetter
    {
        private static readonly Dictionary<Type, Func<object, object>> TypeConverters = new Dictionary<Type, Func<object, object>>();

        public static IGetterSetter Create(PropertyInfo propInf)
        {
            var type = propInf.ReflectedType;
            var propType = propInf.PropertyType;

            var objParam = Expression.Parameter(type, "obj");
            var valueParam = Expression.Parameter(propType, "value");

            var propExpression = Expression.Property(objParam, propInf);

            var getterDelegateType = typeof(Func<,>).MakeGenericType(type, propType);
            var setterDelegateType = typeof(Action<,>).MakeGenericType(type, propType);

            var getterLambda = Expression.Lambda(getterDelegateType, propExpression, objParam);
            var setterLambda = Expression.Lambda(setterDelegateType, Expression.Assign(propExpression, valueParam), objParam, valueParam);

            var getter = getterLambda.Compile();
            var setter = setterLambda.Compile();

            var getterSetterType = typeof(GetterSetterImpl<,>).MakeGenericType(type, propType);
            var getterSetter = Activator.CreateInstance(getterSetterType, new object[] { getter, setter });

            return (IGetterSetter)getterSetter;
        }

        public static IGetterSetter Create<TObject, TProperty>(Expression<Func<TObject, TProperty>> propExp)
        {
            var member = (MemberExpression)propExp.Body;
            var param = Expression.Parameter(typeof(TProperty), "value");
            var lambda = Expression.Lambda<Action<TObject, TProperty>>(
                Expression.Assign(member, param),
                propExp.Parameters[0],
                param);

            var getter = propExp.Compile();
            var setter = lambda.Compile();

            return new GetterSetterImpl<TObject, TProperty>(getter, setter);
        }

        private static Func<object, object> CreateTypeConverter(Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            if (TypeConverters.ContainsKey(type))
            {
                return TypeConverters[type];
            }

            var converter = type.IsEnum
                ? (Func<object, object>)(value => Enum.ToObject(type, value))
                : (value => Convert.ChangeType(value, type));

            TypeConverters.Add(type, converter);

            return converter;
        }

        private class GetterSetterImpl<T, TProp> : IGetterSetter
        {
            private readonly Func<T, TProp> _getter;
            private readonly Action<T, TProp> _setter;
            private readonly Func<object, object> _typeConverter;

            public GetterSetterImpl(Func<T, TProp> getter, Action<T, TProp> setter)
            {
                _getter = getter;
                _setter = setter;
                _typeConverter = CreateTypeConverter(typeof(TProp));
            }

            object IGetter.Get(object obj)
            {
                return _getter((T)obj);
            }

            void ISetter.Set(object obj, object value)
            {
                if (value == null || value is DBNull)
                {
                    return;
                }

                if (value.GetType() != typeof(TProp))
                {
                    value = _typeConverter(value);
                }

                _setter((T)obj, (TProp)value);
            }
        }
    }
}