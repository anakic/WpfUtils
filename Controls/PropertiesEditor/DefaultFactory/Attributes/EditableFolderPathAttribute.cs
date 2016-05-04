using System;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
	/// <summary>
	/// Marks the specified property as an editable folder path. A browse button/OpenFolderDialog
	/// will be used to edit the path.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class EditableFolderPathAttribute : EditableAttribute
    {
        public bool CheckExists { get; set; }

        public EditableFolderPathAttribute(bool checkExists)
        {
            CheckExists = checkExists;
        }
    }
}
