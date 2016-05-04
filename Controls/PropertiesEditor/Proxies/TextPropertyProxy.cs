using System;
using Thingie.WPF.Controls.PropertiesEditor.Proxies.Base;
using System.Globalization;
using System.Windows.Controls;
using System.ComponentModel;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies
{
	public class TextPropertyProxy : EditablePropertyProxy
    {
        public bool Big { get; set; }
        public bool AcceptsReturn { get; set; }
        public bool AcceptsTab { get; set; }

        string _conversionFailString;
        bool _conversionFailedFlag = false;

        public override object Value
        {
            get
            {
                if (_conversionFailedFlag)
                    return _conversionFailString;
                if (base.Value == null)
                    return null;
                else
                    return TypeDescriptor.GetConverter(base.Value).ConvertToString(null, CultureInfo.CurrentCulture, base.Value);
            }
            set
            {
                try 
                {
                    base.Value = TypeDescriptor.GetConverter(Property.PropertyType).ConvertFromString(null, CultureInfo.CurrentCulture, (string)value);
                    _conversionFailedFlag = false;
                }
                catch (Exception ex)
                {
                    _conversionFailString = value as string;
                    _conversionFailedFlag = true;
                    ValidationResult = new ValidationResult(false, ex.Message);
                }
            }
        }

        public override void Cancel()
        {
            _conversionFailedFlag = false;
            base.Cancel();
        }
    }
}
