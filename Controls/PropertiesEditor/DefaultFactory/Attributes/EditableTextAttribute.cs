using System;
using System.ComponentModel;
using System.Globalization;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
    /// <summary>
    /// Marks the specified property as editable plain text
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EditableTextAttribute : EditableAttribute
    {
        public bool Big { get; set; }
        public bool AcceptsReturn { get; set; }
        public bool AcceptsTab { get; set; }

        public Type TextConverterType { get; set; } = typeof(TypeDescriptorTextConverter);
    }

    public interface ITextConverter
    {
        string ToString(object target);
        object FromString(string str, Type t);
    }

    public class TypeDescriptorTextConverter : ITextConverter
    {
        public object FromString(string str, Type t)
            => TypeDescriptor.GetConverter(t).ConvertFromString(null, CultureInfo.CurrentCulture, str);
        public string ToString(object target)
            => TypeDescriptor.GetConverter(target).ConvertToString(null, CultureInfo.CurrentCulture, target);
    }
}
