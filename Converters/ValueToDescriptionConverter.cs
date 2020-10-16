using System;
using System.ComponentModel;
using System.Globalization;

namespace Thingie.WPF.Converters
{
    public class ValueToDescriptionConverter : BaseValueToAttributeConverter
    {
        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            DescriptionAttribute[] attributes = GetAttributes<DescriptionAttribute>(value);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
