using System;
using System.Runtime.InteropServices;
using Thingie.WPF.Controls.PropertiesEditor.CustomEditing;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
	/// <summary>
	/// Specifies the custom editor for a property.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CustomEditorTypeAttribute : Attribute
    {
        public Type EditorType { get; }
        public string VmFactoryMethod { get; }

        public CustomEditorTypeAttribute(Type editorType, string vmFactoryMethod = null)
        {
            if (!typeof(ICustomEditor).IsAssignableFrom(editorType))
                throw new ArgumentException($"Editor type must derive from {nameof(ICustomEditor)}", nameof(editorType));
            EditorType = editorType;
            VmFactoryMethod = vmFactoryMethod;
        }
    }
}
