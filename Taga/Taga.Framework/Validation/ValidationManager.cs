using System;
using System.Collections;
using System.Linq;
using Taga.Framework.Exceptions;

namespace Taga.Framework.Validation
{
    public static class ValidationManager
    {
        private static readonly Hashtable Validators = new Hashtable();

        public static void LoadValidatorsFromAssemblyOf<T>() where T : IValidator
        {
            var validatorTypes = typeof(T).Assembly
                .GetTypes()
                .Where(t => t.BaseType != null && t.BaseType.IsGenericType && t.BaseType.GetGenericTypeDefinition() == typeof(Validator<>));

            foreach (var validatorType in validatorTypes)
            {
                Validators.Add(validatorType.BaseType.GetGenericArguments()[0], Activator.CreateInstance(validatorType));
            }
        }

        public static IValidator GetValidator(Type requestType)
        {
            return Validators[requestType] as IValidator;
        }

        public static void Validate(object value)
        {
            if (value == null)
            {
                throw Errors.F_NullRequest;
            }

            var validator = GetValidator(value.GetType());

            if (validator != null)
            {
                var res = validator.Validate(value);

                if (!res.IsValid)
                {
                    throw res.Error;
                }
            }
        }
    }
}
