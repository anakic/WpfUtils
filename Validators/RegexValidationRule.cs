using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace Thingie.WPF.Validators
{
    public sealed class RegexValidationRule : ValidationRule
    {
        public RegexValidationRule()
        {
            Expression = ".*";
        }

        public string Expression { get; set; }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            bool isOk = Regex.IsMatch(value as string, Expression);
            if (isOk)
                return ValidationResult.ValidResult;
            else
                return new ValidationResult(false, string.Format("Ivalid format. Expected format: {0}", Expression));
        }
    }
}
