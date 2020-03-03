using System;
using System.Globalization;
using System.Windows.Data;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    public class DataTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.GetType().BaseType;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
