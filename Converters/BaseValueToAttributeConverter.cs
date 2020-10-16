using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Data;
using System.Windows.Markup;

namespace Thingie.WPF.Converters
{
    public abstract class BaseValueToAttributeConverter : MarkupExtension, IValueConverter
    {
        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);

        protected T[] GetAttributes<T>(object value) where T : Attribute
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            if (fi != null)
            {
                return (T[])fi.GetCustomAttributes(typeof(T), false);
            }
            else
            {
                PropertyInfo pi = value.GetType().GetProperty(value.ToString());
                if (pi != null)
                {
                    return (T[])pi.GetCustomAttributes(typeof(T), false);
                }
                else
                {
                    return null;
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
