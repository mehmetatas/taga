using System;
using System.Text;

namespace Taga.Core.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetMessages(this Exception ex)
        {
            var sb = new StringBuilder();
            sb.AppendLine(ex.Message);

            while (ex.InnerException != null)
            {
                sb.AppendLine(ex.InnerException.Message);
                ex = ex.InnerException;
            }

            return sb.ToString();
        }
    }
}
