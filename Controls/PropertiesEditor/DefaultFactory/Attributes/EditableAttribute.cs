using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
    /// <summary>
    /// Marks the class/property as editable. An appropriate editor
    /// control will be asigned to the property based on its type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Property, AllowMultiple=false)]
    public class EditableAttribute : ViewableAttribute
    {
    }
}
