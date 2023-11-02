using System;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows;

namespace Thingie.WPF.Controls
{
    [TemplatePart(Name = "PART_Root", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_Background", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_Header", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(FrameworkElement))]
    [TemplatePart(Name = "PART_Content", Type = typeof(FrameworkElement))]
    public class Flyout : HeaderedContentControl
    {
        static Flyout()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Flyout), new FrameworkPropertyMetadata(typeof(Flyout)));
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register("IsOpen", typeof(bool), typeof(Flyout), new PropertyMetadata(false, IsOpenChanged));

        private static void IsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Flyout f = (Flyout)d;
            if (Equals(e.NewValue, true))
                f.ShowFlyout();
            else
                f.HideFlyout();
        }

        void ShowFlyout()
        {
            DoubleAnimation animation = new DoubleAnimation(0, background.ActualWidth, new Duration(TimeSpan.FromMilliseconds(200)))
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                DecelerationRatio = 0.4,
                AccelerationRatio = 0.6,
            };
            content.BeginAnimation(Grid.WidthProperty, animation);
            content.Visibility = Visibility.Visible;
        }

        void HideFlyout()
        {
            DoubleAnimation animation = new DoubleAnimation(background.ActualWidth, 0, new Duration(TimeSpan.FromMilliseconds(200)))
            {
                Duration = new Duration(TimeSpan.FromMilliseconds(200)),
                DecelerationRatio = 0.4,
                AccelerationRatio = 0.6,
            };
            content.BeginAnimation(Grid.WidthProperty, animation);
        }

        FrameworkElement background, content;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            background = (FrameworkElement)GetTemplateChild("PART_Background");
            content = (FrameworkElement)GetTemplateChild("PART_Content");

            if (GetTemplateChild("PART_CloseButton") is Button CloseButton)
            {
                CloseButton.Click += (sender, e) => IsOpen = false;
            }
        }
    }
}
