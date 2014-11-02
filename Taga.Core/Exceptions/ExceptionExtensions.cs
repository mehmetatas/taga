using System;
using System.Text;

namespace Taga.Core.Exceptions
{
    public static class ExceptionExtensions
    {
        public static string GetMessages(this Exception ex)
        {
            var sb = new StringBuilder();

            while (ex != null)
            {
                sb.AppendLine(ex.Message);
                ex = ex.InnerException;
            }

            return sb.ToString();
        }
    }
}
