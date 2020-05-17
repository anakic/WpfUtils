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
                var multiBinding = new MultiBinding
                {
                    Converter = new MultiVisibilityToVisibilityConverter()
                };

                if ((bool)args.NewValue)
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
            else
            {
                groupBox.Loaded += (sender, eventArgs) =>
                {
                    var multiBinding = new MultiBinding
                    {
                        Converter = new MultiVisibilityToVisibilityConverter()
                    };

                    if ((bool)args.NewValue)
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
                };
            }

        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
