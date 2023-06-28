using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Thingie.WPF.Behaviors
{
    public class DialogDragHandleBehavior
    {
        public static readonly DependencyProperty IsDialogDragHandle =
            DependencyProperty.RegisterAttached(nameof(IsDialogDragHandle), typeof(bool), typeof(DialogDragHandleBehavior), new UIPropertyMetadata(false, new PropertyChangedCallback(OnIsDialogDragHandlePropertySet)));

        public static bool GetIsDialogDragHandle(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsDialogDragHandle);
        }

        public static void SetIsDialogDragHandle(DependencyObject obj, bool value)
        {
            obj.SetValue(IsDialogDragHandle, value);
        }

        private static void OnIsDialogDragHandlePropertySet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as FrameworkElement).MouseDown += OnMouseDown;
        }

        private static void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                WindowHelpers.FindHostWindow(sender as FrameworkElement).DragMove();
        }
    }
}
