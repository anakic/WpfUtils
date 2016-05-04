using System.Windows;
using System.Windows.Controls;

namespace Thingie.WPF.Behaviors
{
	public class DataGridBehaviors
    {
        public static int? GetRowHeaderStart(DependencyObject obj)
        {
            return (int?)obj.GetValue(RowHeaderStartProperty);
        }

        public static void SetRowHeaderStart(DependencyObject obj, int? value)
        {
            obj.SetValue(RowHeaderStartProperty, value);
        }

        // Using a DependencyProperty as the backing store for RowHeaderStart.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowHeaderStartProperty =
            DependencyProperty.RegisterAttached("RowHeaderStart", typeof(int?), typeof(DataGridBehaviors), new PropertyMetadata(null, new PropertyChangedCallback(GetRowHeaderStartSet)));


        public static void GetRowHeaderStartSet(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            (source as DataGrid).LoadingRow += DataGridBehaviors_LoadingRow;
        }

        static void DataGridBehaviors_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + (int)(sender as DataGrid).GetValue(RowHeaderStartProperty)).ToString();
        }
    }
}
