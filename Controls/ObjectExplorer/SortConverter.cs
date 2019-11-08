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
            if (value != null)
            {
                var collection = (value as IEnumerable<object>).ToList();
                ListCollectionView view = new ListCollectionView(collection);
                view.SortDescriptions.Add(new SortDescription(nameof(NodeVM.Order), ListSortDirection.Ascending));
                view.SortDescriptions.Add(new SortDescription(nameof(NodeVM.Name), ListSortDirection.Ascending));
                view.Filter = (obj => (obj as NodeVM).IsVisible);
                return view;
            }
            else
                return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
