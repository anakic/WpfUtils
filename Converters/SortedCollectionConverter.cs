using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.Windows.Data;
using System.Collections;

namespace Thingie.WPF.Converters
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public class SortedCollectionConverter: MarkupExtension, IValueConverter
    {
        public SortDirection Direction { get; set; }

        public string Property { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (Direction == SortDirection.Ascending)
                return (value as IEnumerable).OfType<object>().OrderBy(obj => obj.GetType().GetProperty(Property).GetValue(obj, null));
            else
                return (value as IEnumerable).OfType<object>().OrderByDescending(obj => obj.GetType().GetProperty(Property).GetValue(obj, null));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return this;
        }
    }
}
