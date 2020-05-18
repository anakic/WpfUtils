using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Thingie.WPF.Converters;

namespace Thingie.WPF.Behaviors
{
    public class GroupBoxBehaviors
    {
        public static readonly DependencyProperty HideOnChildrenNotVisibleProperty = DependencyProperty.RegisterAttached("HideOnChildrenNotVisible", typeof(bool), typeof(GroupBoxBehaviors), new PropertyMetadata(default(bool), OnHideOnChildrenNotVisibleChanged));
        public static void SetHideOnChildrenNotVisible(GroupBox element, bool value) { element.SetValue(HideOnChildrenNotVisibleProperty, value); }
        public static bool GetHideOnChildrenNotVisible(GroupBox element) { return (bool)element.GetValue(HideOnChildrenNotVisibleProperty); }

        private static void OnHideOnChildrenNotVisibleChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            GroupBox groupBox = obj as GroupBox;

            if (groupBox == null)
                return;

            if (groupBox.IsLoaded)
            {
                SetupVisibilityBinding(groupBox, (bool)args.NewValue);
            }
            else
            {
                groupBox.Loaded += (sender, eventArgs) => SetupVisibilityBinding(groupBox, (bool)args.NewValue);
            }
        }

        private static void SetupVisibilityBinding(GroupBox groupBox, bool newBehaviorPropertyValue)
        {
            var multiBinding = new MultiBinding
            {
                Converter = new MultiVisibilityToVisibilityConverter()
            };

            if (newBehaviorPropertyValue)
            {
                foreach (var child in FindVisualChildren<Button>(groupBox))
                {
                    multiBinding.Bindings.Add(new Binding("Visibility") { Mode = BindingMode.OneWay, Source = child });
                }
                BindingOperations.SetBinding(groupBox, UIElement.VisibilityProperty, multiBinding);
            }
            else
            {
                BindingOperations.ClearBinding(groupBox, UIElement.VisibilityProperty);
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            List<T> childElements = new List<T>();
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    var childVisibility = (Visibility)child.GetValue(UIElement.VisibilityProperty);
                    
                    if (child != null && child is T)
                    {
                        childElements.Add((T)child);
                    }

                    if (VisualTreeHelper.GetChildrenCount(child) > 0 && childVisibility == Visibility.Visible)
                    {
                        childElements.AddRange(FindVisualChildren<T>(child));
                    }
                }
            }

            return childElements;
        }
    }
}
