using System;

namespace Taga.Core.Validation
{
    public static class ValidationUtils
    {
        public static object EnsureType(object obj, Type targetType)
        {
            if (obj == null)
            {
                return null;
            }

            try
            {
                obj = Convert.ChangeType(obj, targetType);
            }
            catch (Exception)
            {
                return null;
            }
            return obj;
        }
    }
}
