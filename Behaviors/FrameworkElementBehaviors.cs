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
        /// A <see cref="DependencyProperty"/> used to store the element's <see cref="FrameworkElement"/> children that impact the element's <see cref="Visibility"/>. This provides a safe and controlled way to dynamically set the element's properties based on the registered children's properties.
        /// </summary>
        public static readonly DependencyProperty VisibilityChildrenDependenciesProperty = 
            DependencyProperty.RegisterAttached("VisibilityChildrenDependencies", typeof(List<FrameworkElement>), typeof(FrameworkElementBehaviors), new PropertyMetadata(null, OnVisibilityChildrenDependenciesChanged));

        /// <summary>
        /// Adds the specified <see cref="FrameworkElement"/> value to the element's <see cref="VisibilityChildrenDependenciesProperty"/>.
        /// </summary>
        /// <param name="element">The element to whose <see cref="VisibilityChildrenDependenciesProperty"/> the <see cref="FrameworkElement"/> value will be added.</param>
        /// <param name="value">The <see cref="FrameworkElement"/> to be added to the <see cref="VisibilityChildrenDependenciesProperty"/>.</param>
        public static void AddVisibilityChildDependency(FrameworkElement element, FrameworkElement value)
        {
            var children = GetVisibilityChildrenDependencies(element);
            // need a copy of the list, otherwise the callback does not get called
            var newChildren = new List<FrameworkElement>(children);
            newChildren.Add(value);
            element.SetValue(VisibilityChildrenDependenciesProperty, newChildren);
        }

        /// <summary>
        /// Gets the <see cref="VisibilityChildrenDependenciesProperty"/> as a <see cref="List{T}"/> of <see cref="FrameworkElement"/>s.
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> from which the <see cref="VisibilityChildrenDependenciesProperty"/> is being retrieved.</param>
        /// <returns>The <see cref="VisibilityChildrenDependenciesProperty"/> as a <see cref="List{T}"/></returns>
        public static List<FrameworkElement> GetVisibilityChildrenDependencies(FrameworkElement element) 
        {
            var collection = (List<FrameworkElement>)element.GetValue(VisibilityChildrenDependenciesProperty);
            if (collection == null)
            {
                collection = new List<FrameworkElement>();
                element.SetValue(VisibilityChildrenDependenciesProperty, collection);
            }
            return collection; 
        }

        /// <summary>
        /// A <see cref="DependencyProperty"/> used to store a registered visibility parent element of a <see cref="FrameworkElement"/>. This enables the parent's <see cref="Visibility"/> property to be binded to the property value of the child.
        /// </summary>
        public static readonly DependencyProperty VisibilityDependentParentProperty =
            DependencyProperty.RegisterAttached("VisibilityDependentParent", typeof(FrameworkElement), typeof(FrameworkElementBehaviors), new UIPropertyMetadata(null, new PropertyChangedCallback(OnVisibilityDependentParentPropertySet)));

        /// <summary>
        /// Gets the <see cref="VisibilityDependentParentProperty"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> whose <see cref="VisibilityDependentParentProperty"/> will be retrieved.</param>
        /// <returns>The <see cref="VisibilityDependentParentProperty"/> as a <see cref="FrameworkElement"/> value.</returns>
        public static FrameworkElement GetVisibilityDependentParent(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(VisibilityDependentParentProperty);
        }

        /// <summary>
        /// Sets the <see cref="VisibilityDependentParentProperty"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the <see cref="VisibilityDependentParentProperty"/> will be set.</param>
        /// <param name="value">The <see cref="FrameworkElement"/> to be set on the <see cref="VisibilityDependentParentProperty"/>.</param>
        public static void SetVisibilityDependentParent(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(VisibilityDependentParentProperty, value);
        }

        /// <summary>
        /// A <see cref="PropertyChangedCallback"/> that handles <see cref="VisibilityDependentParentProperty"/> changes. 
        /// It sets the <see cref="FrameworkElement"/> with the <see cref="VisibilityDependentParentProperty"/> as a parent element's 
        /// visibility dependency that will be taken into consideration when configuring the parent's <see cref="Visibility"/>.
        /// </summary>
        /// <param name="depObj">The <see cref="DependencyObject"/> on which the <see cref="VisibilityDependentParentProperty"/> has changed.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> for <see cref="VisibilityDependentParentProperty"/> changed events.</param>
        private static void OnVisibilityDependentParentPropertySet(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is FrameworkElement child) || !(GetVisibilityDependentParent(depObj) is FrameworkElement parentElement))
                return;

            if (child.IsLoaded)
            {
                FrameworkElementBehaviors.AddVisibilityChildDependency(parentElement, child);
            }
            else
            {
                child.Loaded += (sender, eventArgs) => FrameworkElementBehaviors.AddVisibilityChildDependency(parentElement, child);
            }
        }

        /// <summary>
        /// A <see cref="PropertyChangedCallback"/> that handles <see cref="VisibilityChildrenDependenciesProperty"/> changes.
        /// It sets up a <see cref="FrameworkElement"/>'s <see cref="Visibility"/> property to be binded to the <see cref="Visibility"/> properties of his children dependencies. 
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the <see cref="VisibilityChildrenDependenciesProperty"/> has changed.</param>
        /// <param name="args">The <see cref="DependencyPropertyChangedEventArgs"/> for <see cref="VisibilityChildrenDependenciesProperty"/> changed events.</param>
        private static void OnVisibilityChildrenDependenciesChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
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

            foreach (var child in GetVisibilityChildrenDependencies(element))
            {
                multiBinding.Bindings.Add(new Binding("Visibility") { Mode = BindingMode.OneWay, Source = child });
            }

            BindingOperations.SetBinding(element, UIElement.VisibilityProperty, multiBinding);
        }
    }
}
