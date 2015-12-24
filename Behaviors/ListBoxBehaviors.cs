using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Thingie.WPF.Behaviors
{
    public class ListBoxBehaviors
    {
        public static bool GetAutoScrollLastIntoView(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoScrollLastIntoViewProperty);
        }

        public static void SetAutoScrollLastIntoView(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoScrollLastIntoViewProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoScrollLastIntoView.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoScrollLastIntoViewProperty =
            DependencyProperty.RegisterAttached("AutoScrollLastIntoView", typeof(bool), typeof(ListBoxBehaviors), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoScrollLastIntoViewSet)));

        private static void OnAutoScrollLastIntoViewSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            INotifyCollectionChanged source = listBox.Items as INotifyCollectionChanged;

            var listChangedHandler = new NotifyCollectionChangedEventHandler((s, a) =>
            {
                if ((bool)sender.GetValue(AutoScrollLastIntoViewProperty) == true)
                {
                    if (listBox.Items.Count > 0)
                    {
                        object lastItem = listBox.Items[listBox.Items.Count - 1];
                        listBox.Items.MoveCurrentTo(lastItem);
                        listBox.ScrollIntoView(lastItem);
                    }
                }
            });

            if ((bool)e.NewValue)
                source.CollectionChanged += listChangedHandler;
            else
                source.CollectionChanged -= listChangedHandler;
        }

        private static object IEnumerable(ItemsControl itemsControl)
        {
            throw new NotImplementedException();
        }
    }
}
