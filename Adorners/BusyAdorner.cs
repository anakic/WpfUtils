using Thingie.WPF.Controls;
using System.Windows;
using System.Windows.Documents;

namespace Thingie.WPF.Adorners
{
	public class BusyAdorner : Adorner
    {
        private static readonly DependencyProperty CtrlAdornerProperty = DependencyProperty.RegisterAttached("CtrlAdorner", typeof(BusyAdorner), typeof(BusyAdorner), new UIPropertyMetadata(null));
        public static readonly DependencyProperty ShowAdornerProperty = DependencyProperty.RegisterAttached("ShowAdorner", typeof(bool), typeof(BusyAdorner), new UIPropertyMetadata(false, OnShowAdornerChanged));

        public static bool GetShowAdorner(DependencyObject obj)
        {
            return (bool)obj.GetValue(ShowAdornerProperty);
        }

        public static void SetShowAdorner(DependencyObject obj, bool value)
        {
            obj.SetValue(ShowAdornerProperty, value);
        }

        private static void OnShowAdornerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is FrameworkElement)
            {
                if (e.NewValue != null)
                {
                    FrameworkElement adornedElement = d as FrameworkElement;
                    bool bValue = (bool)e.NewValue;

                    BusyAdorner ctrlAdorner =
                       adornedElement.GetValue(CtrlAdornerProperty) as BusyAdorner;
                    if (ctrlAdorner != null)
                        ctrlAdorner.RemoveLayer();

                    if (bValue)
                    {
                        ctrlAdorner = new BusyAdorner(adornedElement);
                        var adornerLayer = AdornerLayer.GetAdornerLayer(adornedElement);
                        ctrlAdorner.SetLayer(adornerLayer);
                        d.SetValue(CtrlAdornerProperty, ctrlAdorner);
                    }
                }
            }
        }

        private readonly FrameworkElement mAdorningElement;
        private AdornerLayer mLayer;

        public BusyAdorner(FrameworkElement adornedElement)
            : base(adornedElement)
        {
            mAdorningElement = new BusyCtrl();
            AddVisualChild(mAdorningElement);
        }

        protected override int VisualChildrenCount
        {
            get { return mAdorningElement != null ? 1 : 0; }
        }

        protected override System.Windows.Media.Visual GetVisualChild(int index)
        {
            if (index == 0 && mAdorningElement != null)
                return mAdorningElement;

            return base.GetVisualChild(index);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (mAdorningElement != null)
                mAdorningElement.Arrange(new Rect
                (new Point(0, 0), AdornedElement.RenderSize));

            return finalSize;
        }

        public void SetLayer(AdornerLayer layer)
        {
            mLayer = layer;
            mLayer.Add(this);
        }

        public void RemoveLayer()
        {
            if (mLayer != null)
            {
                mLayer.Remove(this);
                RemoveVisualChild(mAdorningElement);
            }
        }
    }
}
