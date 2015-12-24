using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace Thingie.WPF.Behaviors
{
    public class WindowBehaviors
    {
        public static bool GetCancelOnEsc(DependencyObject obj)
        {
            return (bool)obj.GetValue(CancelOnEscProperty);
        }

        public static void SetCancelOnEsc(DependencyObject obj, bool value)
        {
            obj.SetValue(CancelOnEscProperty, value);
        }

        // Using a DependencyProperty as the backing store for CancelOnEsc.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelOnEscProperty =
            DependencyProperty.RegisterAttached("CancelOnEsc", typeof(bool), typeof(Window), new PropertyMetadata(false, new PropertyChangedCallback(CancelOnEscPropertySet)));

        
        private static void CancelOnEscPropertySet(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            Window w = (target as Window);
            w.KeyUp += (s, e) => {
                if (e.Key == System.Windows.Input.Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
                {
                    w.DialogResult = false;
                    w.Close();
                }
                    
            };
        }
    }
}
