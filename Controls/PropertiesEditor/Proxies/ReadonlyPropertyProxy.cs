using System;
using System.Globalization;
using Thingie.WPF.Controls.PropertiesEditor.Proxies.Base;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies
{
	class ReadonlyPropertyProxy : PropertyProxy
    {
        public override object Value
        {
            get
            {
                if (base.Value == null)
                    return null;
                else
                    return Convert.ChangeType(base.Value, typeof(string), CultureInfo.CurrentCulture);
            }
            set
            {
                base.Value = value;
            }
        }
    }
}
