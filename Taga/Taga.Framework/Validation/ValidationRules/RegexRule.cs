using System.Text.RegularExpressions;

namespace Taga.Framework.Validation.ValidationRules
{
    public class RegexRule : IValidationRule
    {
        private readonly Regex _regex;

        public RegexRule(string pattern, RegexOptions options =  RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
        {
            _regex = new Regex(pattern, options);
        }

        public bool Execute(object obj)
        {
            var str = obj as string;

            if (str == null)
            {
                return false;
            }
            
            return _regex.IsMatch(str);
        }
    }
}
