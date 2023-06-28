using System.Windows;
using System.Windows.Media;

namespace Thingie.WPF
{
    internal static class WindowHelpers
    {
        public static Window FindHostWindow(FrameworkElement control)
        {
            if (control is Window)
                return (control as Window);
            else
                return FindHostWindow((FrameworkElement)VisualTreeHelper.GetParent(control));
        }
    }
}