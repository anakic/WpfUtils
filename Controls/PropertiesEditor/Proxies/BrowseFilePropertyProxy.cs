using Thingie.WPF.Controls.PropertiesEditor.Proxies.Base;
using System.Windows.Controls;
using System.IO;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies
{
	public class BrowseFilePropertyProxy : EditablePropertyProxy
    {
        public bool CheckFileExists { get; set; }

        public string Filter { get; set; }

        protected override ValidationResult CheckValidity(object val)
        {
            if (CheckFileExists && !File.Exists(val as string))
                return new ValidationResult(false, "File does not exist!");
            //else if(Filter != "*" && !Filter.EndsWith("|*.*"))
            //{
            //    Match m = Regex.Match(Filter, @"^((.*\|)?\*\.(?'ext'[^;]*);?)*$");
            //    bool success = false;
            //    if (val == null)
            //        success = true;
            //    else
            //    {
            //        foreach (Capture capt in m.Groups["ext"].Captures)
            //        {
            //            if ((val as string).EndsWith(capt.Value))
            //            {
            //                success = true;
            //                break;
            //            }
            //        }
            //    }
            //    if(!success)
            //        return new ValidationResult(false, "Invalid file type!");
            //    else
            //        return base.CheckValidity(val);
            //}
            else
                return base.CheckValidity(val);
        }
    }
}
