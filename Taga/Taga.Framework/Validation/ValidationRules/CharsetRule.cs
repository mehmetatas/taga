using System;
using System.Globalization;
using System.Linq;

namespace Taga.Framework.Validation.ValidationRules
{
    public class CharsetRule : IValidationRule
    {
        private readonly string _charset;
        private readonly bool _caseSensitive;
        private readonly CultureInfo _culture;

        public CharsetRule(string charset, bool caseSensitive, string culture)
        {
            _caseSensitive = caseSensitive;
            _culture = new CultureInfo(culture);

            _charset = _caseSensitive ? charset : charset.ToLower(_culture);
        }

        public bool Execute(object obj)
        {
            var str = obj as string;

            if (String.IsNullOrEmpty(str))
            {
                return false;
            }

            if (!_caseSensitive)
            {
                str = str.ToLower(_culture);
            }

            return str.All(c => _charset.IndexOf(c) >= 0);
        }
    }
}
