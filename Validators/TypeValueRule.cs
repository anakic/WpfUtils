using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using Thingie.WPF.Resources;
using System.Globalization;

namespace Thingie.WPF.Validators
{
    public class TypeValueRule : ValidationRule
    {
        public Type DestinationType { get; set; }

        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            try
            {
                Convert.ChangeType(value, DestinationType, CultureInfo.CurrentCulture);
                return ValidationResult.ValidResult;
            }
            catch (Exception ex)
            {
                return new ValidationResult(false, string.Format(ex.Message, value, DestinationType.Name));
            }
        }
    }
}
