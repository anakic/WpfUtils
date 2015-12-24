using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace Thingie.WPF.Behaviors
{
    public class ButtonBehaviors
    {
        public static bool GetFocusOnClick(DependencyObject obj)
        {
            return (bool)obj.GetValue(FocusOnClickProperty);
        }

        public static void SetFocusOnClick(DependencyObject obj, bool value)
        {
            obj.SetValue(FocusOnClickProperty, value);
        }

        // Using a DependencyProperty as the backing store for FocusOnClick.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FocusOnClickProperty =
            DependencyProperty.RegisterAttached("FocusOnClick", typeof(bool), typeof(ButtonBehaviors), new UIPropertyMetadata(false, new PropertyChangedCallback(OnFocusOnClickPropertySet)));

        private static void OnFocusOnClickPropertySet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as Button).Click += new RoutedEventHandler(ButtonBehaviors_Click);
        }

        static void ButtonBehaviors_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).Focus();
        }
    }
}
