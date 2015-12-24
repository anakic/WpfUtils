using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
    /// <summary>
    /// Marks the specified attribute as editable via the specified custom control type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class EditableCustomAttribute : EditableAttribute
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="controlType">
        /// The type of the control used to edit the property. When needed, the control
        /// will be instantiated and the value of the property passed to it through it's
        /// DataContext property.
        /// </param>
        public EditableCustomAttribute(Type controlType)
        {
            ControlType = controlType;
        }

        public Type ControlType { get; private set; }
    }
}
