using System;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
	/// <summary>
	/// Marks the specified type (T) as editable via a custom editor control. Use this attribute
	/// if you create a custom type T that needs to be edited using the property grid control, and you
	/// want to avoid specifying the same custom editor every time you define a property of type T
	/// that you want to be editable. 
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TypeCustomEditorAttribute : Attribute
    {
        public Type EditorType { get; set; }

        public TypeCustomEditorAttribute(Type editorType)
        {
            EditorType = editorType;
        }
    }
}
