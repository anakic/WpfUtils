using System;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
	/// <summary>
	/// Marks the class/property as editable. An appropriate editor
	/// control will be asigned to the property based on its type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class|AttributeTargets.Property, AllowMultiple=false)]
    public class EditableAttribute : ViewableAttribute
    {
        public string When { get; set; }
    }
}
