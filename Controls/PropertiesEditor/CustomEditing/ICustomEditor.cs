using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thingie.WPF.Controls.PropertiesEditor.CustomEditing
{
    public interface ICustomEditor
    {
        object Value { get; set; }
    }
}
