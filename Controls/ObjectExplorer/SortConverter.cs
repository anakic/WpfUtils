using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace Thingie.WPF.Controls.ObjectExplorer
{
    public class SortConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var oc = value as ObservableCollection<NodeVM>;

            if (value != null)
            {
                var collection = (value as IList);
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
