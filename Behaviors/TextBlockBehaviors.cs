using System;
using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Text.RegularExpressions;

namespace Thingie.WPF.Behaviors
{
    public static class TextBlockBehaviors
    {
        public static readonly DependencyProperty HtmlContentProperty = DependencyProperty.RegisterAttached(
            "HtmlContent",
            typeof(string),
            typeof(TextBlockBehaviors),
            new PropertyMetadata(null, OnHtmlContentChanged));

        public static void SetHtmlContent(UIElement element, string value)
        {
            element.SetValue(HtmlContentProperty, value);
        }

        public static string GetHtmlContent(UIElement element)
        {
            return (string)element.GetValue(HtmlContentProperty);
        }

        private static void OnHtmlContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock textBlock)
            {
                string inputString = (string)e.NewValue;

                // Here, add your logic to convert the string to a list of Inlines
                List<Inline> inlines = ConvertStringToInlines(inputString);

                textBlock.Inlines.Clear();
                foreach (var inline in inlines)
                {
                    textBlock.Inlines.Add(inline);
                }
            }
        }

        private static List<Inline> ConvertStringToInlines(string input)
        {
            var inlines = new List<Inline>();
            var regex = new Regex(@"(<a href='(?<url>[^']+)'>|<b>)(?<text>.*?)(</a>|</b>)", RegexOptions.IgnoreCase);

            int lastIndex = 0;
            foreach (Match match in regex.Matches(input))
            {
                // Add the text before the tag as a Run
                if (match.Index > lastIndex)
                {
                    inlines.Add(new Run(input.Substring(lastIndex, match.Index - lastIndex)));
                }

                var tagContent = match.Groups["text"].Value;

                if (match.Value.StartsWith("<a"))
                {
                    string url = match.Groups["url"].Value;
                    var hyperlink = new Hyperlink
                    {
                        NavigateUri = new Uri(url),
                        Inlines = { new Run(tagContent) }
                    };
                    // Add Click event handler if needed
                    // hyperlink.Click += YourClickEventHandler;

                    inlines.Add(hyperlink);
                }
                else if (match.Value.StartsWith("<b"))
                {
                    var boldRun = new Run(tagContent) { FontWeight = FontWeights.Bold };
                    inlines.Add(boldRun);
                }

                lastIndex = match.Index + match.Length;
            }

            // Add any remaining text after the last tag
            if (lastIndex < input.Length)
            {
                inlines.Add(new Run(input.Substring(lastIndex)));
            }

            return inlines;
        }

        public static readonly DependencyProperty DesiredWidthProperty =
            DependencyProperty.RegisterAttached(
            "DesiredWidth",
            typeof(double),
            typeof(TextBlockBehaviors),
            new PropertyMetadata((double)0));

        public static readonly DependencyProperty ShowTooltipIfTrimmedProperty =
            DependencyProperty.RegisterAttached(
            "ShowTooltipIfTrimmed",
            typeof(bool),
            typeof(TextBlockBehaviors),
            new PropertyMetadata(false, OnShowTooltipIfTrimmedChanged));

        public static bool GetShowTooltipIfTrimmed(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowTooltipIfTrimmedProperty);
        }

        public static void SetShowTooltipIfTrimmed(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowTooltipIfTrimmedProperty, value);
        }

        private static void OnShowTooltipIfTrimmedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBlock tb)
            {
                tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
                var width = tb.DesiredSize.Width;
                tb.SetValue(DesiredWidthProperty, width);

                if ((bool)e.NewValue)
                {
                    tb.SizeChanged += TextBox_SizeChanged;
                }
                else
                {
                    tb.SizeChanged -= TextBox_SizeChanged;
                }
            }
        }

        private static void TextBox_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var tb = sender as TextBlock;

            var width = (double)tb.GetValue(DesiredWidthProperty);

            if (tb.ActualWidth < width)
            {
                ToolTipService.SetToolTip(tb, tb.Text);
            }
            else
            {
                ToolTipService.SetToolTip(tb, null);
            }
        }
    }
}
