using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Thingie.WPF.Controls
{
    /// <summary>
    /// Interaction logic for ProgressRing.xaml
    /// </summary>
    public partial class ProgressRing : UserControl
    {
        private readonly DispatcherTimer _spinnerTimer;

        // the nominal size of the spinner - the actual size is determined by the Width / Height as the spinner is contained within a ViewBox
        public double Diameter => 100.0;
        public double ItemDiameter => Diameter / 6.0;
        public double ItemRadius => ItemDiameter / 2.0;
        public double ItemPositionRadius => (Diameter - ItemDiameter) / 2.0;

        public ProgressRing()
        {
            InitializeComponent();

            this.Foreground = new SolidColorBrush(Color.FromRgb(0x34,0x7B,0xB1));

            _spinnerTimer = new DispatcherTimer(DispatcherPriority.Normal, Dispatcher);
            _spinnerTimer.Tick += (s, e) => SpinnerRotateTransform.Angle = (SpinnerRotateTransform.Angle + 30) % 360;

            Loaded += (s, e) => OnLoaded();
            Unloaded += (s, e) => Stop();
            IsVisibleChanged += (s, e) => OnIsVisibleChanged((bool)e.NewValue);
        }

        /// <summary>
        /// IsVisibleChanged also covers the case where the spinner is placed inside another control which itself is collapsed or hidden
        /// </summary>
        /// <param name="isVisible">
        /// </param>
        private void OnIsVisibleChanged(bool isVisible)
        {
            if (isVisible)
                Start();
            else
                Stop();
        }

        /// <summary>
        /// Rotations per minute
        /// </summary>
        public int Speed
        {
            get => (int)GetValue(SpeedProperty);
            set => SetValue(SpeedProperty, value);
        }

        public static readonly DependencyProperty SpeedProperty =
            DependencyProperty.Register("Speed", typeof(int), typeof(ProgressRing), new PropertyMetadata(60));

        private void OnLoaded()
        {
            SetItemPosition(Item1, 0);
            SetItemPosition(Item2, 1);
            SetItemPosition(Item3, 2);
            SetItemPosition(Item4, 3);
            SetItemPosition(Item5, 4);
            SetItemPosition(Item6, 5);
            SetItemPosition(Item7, 6);
            SetItemPosition(Item8, 7);
            SetItemPosition(Item9, 8);
            SetItemPosition(Item10, 9);
            SetItemPosition(Item11, 10);
            SetItemPosition(Item12, 11);
        }

        private void SetItemPosition(DependencyObject item, int index)
        {
            item.SetValue(Canvas.LeftProperty, Diameter / 2.0 + (Math.Sin(Math.PI * (index / 6.0)) * ItemPositionRadius) - ItemRadius);
            item.SetValue(Canvas.TopProperty, Diameter / 2.0 + (Math.Cos(Math.PI * (index / 6.0)) * ItemPositionRadius) - ItemRadius);
        }

        private void Stop()
        {
            _spinnerTimer.Stop();
        }

        private void Start()
        {
            // each tick of the timer is 1 step of revolution
            _spinnerTimer.Interval = TimeSpan.FromMilliseconds(60000 / (12.0 * Speed));
            _spinnerTimer.Start();
        }
    }
}
