using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
    /// <summary>
    /// Marks the specified property as editable plain text
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class EditableTextAttribute : EditableAttribute
    {
        public bool Big { get; set; }
        public bool AcceptsReturn { get; set; }
        public bool AcceptsTab { get; set; }
    }
}
