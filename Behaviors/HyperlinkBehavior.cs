using System.Windows;
using System.Windows.Documents;

namespace Thingie.WPF.Behaviors
{
	public static class HyperlinkBehavior
    {
        public static bool GetLaunchesDefaultBrowser(DependencyObject obj)
        {
            return (bool)obj.GetValue(LaunchesDefaultBrowserProperty);
        }

        public static void SetLaunchesDefaultBrowser(DependencyObject obj, bool value)
        {
            obj.SetValue(LaunchesDefaultBrowserProperty, value);
        }
        
        public static readonly DependencyProperty LaunchesDefaultBrowserProperty =
            DependencyProperty.RegisterAttached("LaunchesDefaultBrowser", typeof(bool), typeof(Hyperlink), new PropertyMetadata(false, new PropertyChangedCallback(LaunchesDefaultBrowserSet)));

        private static void LaunchesDefaultBrowserSet(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if ((bool)args.NewValue)
            {
                ((Hyperlink)target).RequestNavigate += (s, e) => System.Diagnostics.Process.Start(e.Uri.ToString());
            }
        }
    }
}
