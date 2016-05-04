using System;
using System.Windows.Data;
using System.Windows.Markup;
using System.ComponentModel;

namespace Thingie.WPF.Converters
{
	public class StringTypeConverter : MarkupExtension, IValueConverter
    {
        #region IValueConverter Members

        TypeConverter _converter;

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
                return null;

            if(_converter==null)
                _converter = TypeDescriptor.GetConverter(value);
            return _converter.ConvertToString(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value.GetType() != typeof(string))
                throw new ArgumentException("Target property for this converter must be of type string.");

            object returnValue;

            if (_converter == null)
                returnValue = value as string;
            else
            {
                try 
                {
                    returnValue = _converter.ConvertFromString(value as string);
                }
                catch 
                {
                    //if conversion fails, return the unconveted value, which should
                    //fail in binding, and trigger the ValidateOnExceptions...
                    returnValue = value;
                }
            }

            return returnValue;
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
