using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Thingie.WPF.Converters;

namespace Thingie.WPF.Behaviors
{
    /// <summary>
    /// Represents behaviors that can be applied to a <see cref="FrameworkElement"/>.
    /// </summary>
    public class FrameworkElementBehaviors
    {
        /// <summary>
        /// A <see cref="DependencyProperty"/> used to store the element's <see cref="FrameworkElement"/> children. This provides a safe and controlled way to dynamically set the element's properties based on the registered children's properties.
        /// </summary>
        public static readonly DependencyProperty RegisteredChildrenProperty = 
            DependencyProperty.RegisterAttached("RegisteredChildren", typeof(List<FrameworkElement>), typeof(FrameworkElementBehaviors), new PropertyMetadata(null, OnRegisteredChildrenChanged));

        /// <summary>
        /// Adds the specified <see cref="FrameworkElement"/> value to the element's <see cref="RegisteredChildrenProperty"/>.
        /// </summary>
        /// <param name="element">The element to whose <see cref="RegisteredChildrenProperty"/> the <see cref="FrameworkElement"/> value will be added.</param>
        /// <param name="value">The <see cref="FrameworkElement"/> to be added to the <see cref="RegisteredChildrenProperty"/>.</param>
        public static void AddRegisteredChild(FrameworkElement element, FrameworkElement value)
        {
            var children = GetRegisteredChildren(element);
            // need a copy of the list, otherwise the callback does not get called
            var newChildren = new List<FrameworkElement>(children);
            newChildren.Add(value);
            element.SetValue(RegisteredChildrenProperty, newChildren);
        }

        /// <summary>
        /// Gets the <see cref="RegisteredChildrenProperty"/> as a <see cref="List{T}"/> of <see cref="FrameworkElement"/>s.
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> from which the <see cref="RegisteredChildrenProperty"/> is being retrieved.</param>
        /// <returns>The <see cref="RegisteredChildrenProperty"/> as a <see cref="List{T}"/></returns>
        public static List<FrameworkElement> GetRegisteredChildren(FrameworkElement element) 
        {
            var collection = (List<FrameworkElement>)element.GetValue(RegisteredChildrenProperty);
            if (collection == null)
            {
                collection = new List<FrameworkElement>();
                element.SetValue(RegisteredChildrenProperty, collection);
            }
            return collection; 
        }

        /// <summary>
        /// A <see cref="DependencyProperty"/> used to store a registered parent element of a <see cref="FrameworkElement"/>. This enables the parent's properties to be binded to the property values of the child.
        /// </summary>
        public static readonly DependencyProperty RegisteredParentProperty =
            DependencyProperty.RegisterAttached("RegisteredParent", typeof(FrameworkElement), typeof(FrameworkElementBehaviors), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRegisteredParentPropertySet)));

        /// <summary>
        /// Gets the <see cref="RegisteredParentProperty"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> whose <see cref="RegisteredParentProperty"/> will be retrieved.</param>
        /// <returns>The <see cref="RegisteredParentProperty"/> as a <see cref="FrameworkElement"/> value.</returns>
        public static FrameworkElement GetRegisteredParent(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(RegisteredParentProperty);
        }

        /// <summary>
        /// Sets the <see cref="RegisteredParentProperty"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the <see cref="RegisteredParentProperty"/> will be set.</param>
        /// <param name="value">The <see cref="FrameworkElement"/> to be set on the <see cref="RegisteredParentProperty"/>.</param>
        public static void SetRegisteredParent(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(RegisteredParentProperty, value);
        }

        /// <summary>
        /// A <see cref="PropertyChangedCallback"/> that handles <see cref="RegisteredParentProperty"/> changes. 
        /// It sets the <see cref="FrameworkElement"/> with the <see cref="RegisteredParentProperty"/> as a parent element's 
        /// registered child that will be taken into consideration when configuring the parent's properties.
        /// </summary>
        /// <param name="depObj">The <see cref="DependencyObject"/> on which the <see cref="RegisteredParentProperty"/> has changed.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> for <see cref="RegisteredParentProperty"/> changed events.</param>
        private static void OnRegisteredParentPropertySet(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is FrameworkElement child) || !(GetRegisteredParent(depObj) is FrameworkElement parentElement))
                return;

            if (child.IsLoaded)
            {
                FrameworkElementBehaviors.AddRegisteredChild(parentElement, child);
            }
            else
            {
                child.Loaded += (sender, eventArgs) => FrameworkElementBehaviors.AddRegisteredChild(parentElement, child);
            }
        }

        /// <summary>
        /// A <see cref="PropertyChangedCallback"/> that handles <see cref="RegisteredChildrenProperty"/> changes.
        /// It sets up a <see cref="FrameworkElement"/>s properties to be binded to its registered children properties. 
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the <see cref="RegisteredChildrenProperty"/> has changed.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> for <see cref="RegisteredChildrenProperty"/> changed events.</param>
        private static void OnRegisteredChildrenChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (!(obj is FrameworkElement element))
                return;

            if (element.IsLoaded)
            {
                SetupVisibilityBinding(element);
            }
            else
            {
                element.Loaded += (sender, eventArgs) => SetupVisibilityBinding(element);
            }
        }

        /// <summary>
        /// Adds and optionally clears the <see cref="Visibility"/> property binding on a <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> on which the binding will be added and optionally removed.</param>
        private static void SetupVisibilityBinding(FrameworkElement element)
        {
            var multiBinding = new MultiBinding
            {
                Converter = new MultiVisibilityToVisibilityConverter()
            };

            bool isBindingSet = element.GetBindingExpression(UIElement.VisibilityProperty) != null ? true : false;

            if (isBindingSet)
            {
                BindingOperations.ClearBinding(element, UIElement.VisibilityProperty);
            }

            foreach (var child in GetRegisteredChildren(element))
            {
                multiBinding.Bindings.Add(new Binding("Visibility") { Mode = BindingMode.OneWay, Source = child });
            }

            BindingOperations.SetBinding(element, UIElement.VisibilityProperty, multiBinding);
        }
    }
}
