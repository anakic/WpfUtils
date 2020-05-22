using System;
using Thingie.WPF.Controls.PropertiesEditor.Proxies.Base;
using System.Globalization;
using System.Windows.Controls;
using System.ComponentModel;
using Thingie.WPF.Controls.PropertiesEditor.DefaultFactory.Attributes;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies
{
	public class TextPropertyProxy : EditablePropertyProxy
    {
        public bool Big { get; set; }
        public bool AcceptsReturn { get; set; }
        public bool AcceptsTab { get; set; }

        public ITextConverter TextConverter { get; set; } = new TypeDescriptorTextConverter();

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
                    return TextConverter.ToString(base.Value);
            }
            set
            {
                try 
                {
                    base.Value = TextConverter.FromString((string)value, Property.PropertyType);
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
