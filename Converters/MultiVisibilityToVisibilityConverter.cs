using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Thingie.WPF.Converters
{
    public class MultiVisibilityToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.OfType<Visibility>().Any(vis => vis != Visibility.Collapsed) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
