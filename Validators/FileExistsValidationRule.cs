using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.IO;

namespace Thingie.WPF.Validators
{
    public class FileExistsValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if ((value is string) == false)
                throw new ArgumentException();

            string valString = value as string;
            if (string.IsNullOrEmpty(valString))
                return ValidationResult.ValidResult;
            else
            {
                if (File.Exists(valString))
                    return ValidationResult.ValidResult;
                else
                    return new ValidationResult(false, "File does not exist");
            }
        }
    }
}
