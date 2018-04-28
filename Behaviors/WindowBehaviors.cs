using System.Windows;
using System.Windows.Input;

namespace Thingie.WPF.Behaviors
{
    public interface ICancelOnEscWindow
    {
        bool CanCancelOnEsc();
    }

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
            if(target is Window w)
            {
                w.KeyUp += (s, e) =>
                {
                    if (e.Key == System.Windows.Input.Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
                    {
                        if (w is ICancelOnEscWindow == false || w is ICancelOnEscWindow cew && cew.CanCancelOnEsc())
                        {
                            try
                            {
                                w.DialogResult = false;
                            }
                            catch
                            {
                                //will fail if modal, but there's no way to determine if it is modal so using try-catch
                            }
                            w.Close();
                        }
                    }

                };
            }
        }
    }
}
