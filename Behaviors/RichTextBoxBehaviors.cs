using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows;

namespace Thingie.WPF.Behaviors
{
    public class RichTextBoxBehaviors
    {
        public static string GetRTF(DependencyObject obj)
        {
            return (string)obj.GetValue(RTFProperty);
        }

        public static void SetRTF(DependencyObject obj, string value)
        {
            obj.SetValue(RTFProperty, value);
        }

        // Using a DependencyProperty as the backing store for RTF.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RTFProperty =
            DependencyProperty.RegisterAttached("RTF", typeof(byte[]), typeof(RichTextBoxBehaviors), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRTFSet)) { BindsTwoWayByDefault = true });

        static object ignoreSender;
        private static void OnRTFSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender == ignoreSender)
                return;

            MemoryStream ms = new MemoryStream(e.NewValue as byte[]);
            RichTextBox richTextBox = (RichTextBox)sender;

            TextRange range = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            range.Load(ms, System.Windows.DataFormats.Rtf);
            ms.Close();

            ((RichTextBox)sender).LostFocus += (obj, args) =>
            {
                ignoreSender = sender;
                MemoryStream ms1 = new MemoryStream();
                range.Save(ms1, DataFormats.Rtf);
                sender.SetValue(RTFProperty, ms1.GetBuffer());
                ms1.Close();
                ignoreSender = null;
            };
        }
    }
}
