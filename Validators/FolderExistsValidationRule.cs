using System.Windows.Controls;
using System.IO;

namespace Thingie.WPF.Validators
{
	public class FolderExistsValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {
            if (Directory.Exists((string)value))
                return ValidationResult.ValidResult;
            else
                return new ValidationResult(false, "Folder does not exist!");
        }
    }
}
