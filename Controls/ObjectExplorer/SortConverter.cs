using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    public class SortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = (value as IEnumerable<object>).ToList();
            ListCollectionView view = new ListCollectionView(collection);
            view.SortDescriptions.Add(new SortDescription("Order", ListSortDirection.Ascending));
            view.SortDescriptions.Add(new SortDescription("Text", ListSortDirection.Ascending));
            return view;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
