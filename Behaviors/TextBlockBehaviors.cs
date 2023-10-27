using System;
using System.Windows.Controls;
using System.Windows;

namespace Thingie.WPF.Behaviors
{
    public static class TextBlockBehaviors
    {
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

            tb.Measure(new Size(Double.PositiveInfinity, Double.PositiveInfinity));
            var width = tb.DesiredSize.Width;

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
