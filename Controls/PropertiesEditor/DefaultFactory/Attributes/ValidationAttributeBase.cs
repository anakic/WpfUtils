using System;
using System.Windows.Controls;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
	/// <summary>
	/// Specifies the validation logic for the specified property.
	/// </summary>
	public abstract class ValidationAttributeBase : Attribute
    {
        public abstract ValidationResult Validate(object obj);
    }
}
