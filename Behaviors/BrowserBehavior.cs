using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Thingie.WPF.Behaviors
{
    public class BrowserBehavior
    {
        public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached(
                "Html",
                typeof(Task<string>),
                typeof(BrowserBehavior),
                new FrameworkPropertyMetadata(OnHtmlChanged));

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public static Task<string> GetHtml(WebBrowser d)
        {
            return (Task<string>)d.GetValue(HtmlProperty);
        }

        public static void SetHtml(WebBrowser d, Task<string> value)
        {
            d.SetValue(HtmlProperty, value);
        }

        static async void OnHtmlChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser webBrowser = dependencyObject as WebBrowser;
            if (webBrowser != null)
            {
                webBrowser.NavigateToString("Loading...");

                var htmlTask = e.NewValue as Task<string>;
                var html = await htmlTask;
                await webBrowser.Dispatcher.BeginInvoke(new Action(() => webBrowser.NavigateToString(string.IsNullOrEmpty(html) ? "&nbsp;" : html)));
            }
        }
    }

}
