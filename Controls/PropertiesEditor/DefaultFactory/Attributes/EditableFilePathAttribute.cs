using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes
{
    /// <summary>
    /// Marks the specified property as an editable file path. A browse button/OpenFileDialog
    /// will be used to edit the path.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class EditableFilePathAttribute : EditableAttribute
    {
        public bool CheckExists { get; set; }

        private string _Filter = "All files|*.*";
        public string Filter
        {
            get
            {
                if (_Filter == null)
                    _Filter = string.Empty;
                return _Filter;
            }
            set
            {
                _Filter = value;
            }
        }

        public EditableFilePathAttribute(bool checkFileExists)
        {
            CheckExists = checkFileExists;
        }
    }
}
