using System;
using System.Windows.Controls;
using System.Text.RegularExpressions;
using System.Collections;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
	/// <summary>
	/// Marks the specified property as mandatory. An empty value will be treated as invalid.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class RequiredAttribute : ValidationAttributeBase
    {
        public override ValidationResult Validate(object obj)
        {
            bool isValid;
            if(obj is string)
                isValid = !string.IsNullOrEmpty(obj as string);
            else
                isValid = (obj != null);

            if (isValid)
                return ValidationResult.ValidResult;
            else
                return new ValidationResult(false, "Required field");
        }
    }

    /// <summary>
    /// Specifies that a collection property is invalid if empty.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CollectionNotEmptyAttribute : ValidationAttributeBase
    {
        public CollectionNotEmptyAttribute()
        {
        }

        public override ValidationResult Validate(object obj)
        {
            ICollection objAsIEnumberable = obj as ICollection;

            if ((objAsIEnumberable != null)&&(objAsIEnumberable.Count>0))
                return ValidationResult.ValidResult;
            else
                return new ValidationResult(false, "List must not be empty!");
        }
    }

    /// <summary>
    /// Specifies the regular expressio the property must match in order to be valid.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RegexAttribute : ValidationAttributeBase
    {
        string _pattern;

        public RegexAttribute(string pattern)
        {
            _pattern = pattern;
        }

        public override ValidationResult Validate(object obj)
        {
            string objAsString = ((string)obj)??string.Empty;

            if (Regex.IsMatch(objAsString, _pattern))
                return ValidationResult.ValidResult;
            else
                return new ValidationResult(false, "Invalid format");

        }
    }

    /// <summary>
    /// Specifies the valid bounds of a numeric attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class RangeAttribute : ValidationAttributeBase
    {
        double _min, _max;
        bool _includeMin, _includeMax;

        public RangeAttribute(double min, double max, bool includeMin, bool includeMax)
        {
            _min = min;
            _max = max;
            _includeMax = includeMax;
            _includeMin = includeMin;
        }

        #region IValidationAttribute Members

        public override ValidationResult Validate(object obj)
        {
            double objAsDec = Convert.ToDouble(obj);
            if ((objAsDec > _min) && (objAsDec < _max))
                return ValidationResult.ValidResult;
            else if ((objAsDec == _min) && _includeMin)
                return ValidationResult.ValidResult;
            else if ((objAsDec == _max) && _includeMax)
                return ValidationResult.ValidResult;
            else
                return new ValidationResult(false, "Out of range!");
        }

        #endregion
    }
}
