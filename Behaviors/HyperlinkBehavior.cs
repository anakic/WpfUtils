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
            DependencyProperty.RegisterAttached("LaunchesDefaultBrowser", typeof(bool), typeof(HyperlinkBehavior), new PropertyMetadata(false, new PropertyChangedCallback(LaunchesDefaultBrowserSet)));

        private static void LaunchesDefaultBrowserSet(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if ((bool)args.NewValue)
            {
                ((Hyperlink)target).RequestNavigate += (s, e) => System.Diagnostics.Process.Start(e.Uri.ToString());
            }
        }

        public static bool GetUnderlineOnlyOnMouseEnter(DependencyObject obj)
        {
            return (bool)obj.GetValue(UnderlineOnlyOnMouseEnterProperty);
        }

        public static void SetUnderlineOnlyOnMouseEnter(DependencyObject obj, bool value)
        {
            obj.SetValue(UnderlineOnlyOnMouseEnterProperty, value);
        }

        public static readonly DependencyProperty UnderlineOnlyOnMouseEnterProperty =
            DependencyProperty.RegisterAttached("UnderlineOnlyOnMouseEnter", typeof(bool), typeof(Hyperlink), new PropertyMetadata(false, new PropertyChangedCallback(UnderlineOnlyOnMouseEnterPropertySet)));

        private static void UnderlineOnlyOnMouseEnterPropertySet(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            if ((bool)args.NewValue)
            {
                var hyperLink = (Hyperlink)target;
                hyperLink.TextDecorations = null;
                hyperLink.MouseEnter += (s, e) => hyperLink.TextDecorations = TextDecorations.Underline;
                hyperLink.MouseLeave += (s, e) => hyperLink.TextDecorations = null;
            }
        }
    }
}
