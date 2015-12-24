using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
    public sealed class EditableShortcutAttribute : EditableAttribute
    {
        public string ModifiersProperty { get; set; }
    }
}
