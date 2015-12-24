using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thingie.WPF.Controls.PropertiesEditor.Proxies.Base;
using System.IO;
using System.Windows.Controls;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies
{
    public class BrowseFolderPropertyProxy : EditablePropertyProxy
    {
        public bool CheckFolderExists { get; set; }

        protected override ValidationResult CheckValidity(object val)
        {
            if (CheckFolderExists && !Directory.Exists(val as string))
                return new ValidationResult(false, "Folder does not exist!");
            else
                return base.CheckValidity(val);
        }
    }
}
