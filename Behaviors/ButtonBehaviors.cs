using System.Windows;
using System.Windows.Controls;

namespace Thingie.WPF.Behaviors
{
    /// <summary>
    /// Represents behaviors that can be applied to a <see cref="Button"/>.
    /// </summary>
	public class ButtonBehaviors
    {
        /// <summary>
        /// A <see cref="DependencyProperty"/> used to provide the FocusOnClick behavior. This in turn enables animation, styling, binding etc.
        /// </summary>
        public static readonly DependencyProperty FocusOnClickProperty =
            DependencyProperty.RegisterAttached("FocusOnClick", typeof(bool), typeof(ButtonBehaviors), new UIPropertyMetadata(false, new PropertyChangedCallback(OnFocusOnClickPropertySet)));

        /// <summary>
        /// Gets the <see cref="FocusOnClickProperty"/> as a <see cref="bool"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> whose <see cref="FocusOnClickProperty"/> will be retrieved.</param>
        /// <returns>The <see cref="FocusOnClickProperty"/> as a <see cref="bool"/>.</returns>
        public static bool GetFocusOnClick(DependencyObject obj)
        {
            return (bool)obj.GetValue(FocusOnClickProperty);
        }

        /// <summary>
        /// Sets the <see cref="FocusOnClickProperty"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyObject"/> on which the <see cref="FocusOnClickProperty"/> will be set.</param>
        /// <param name="value">The <see cref="bool"/> value to be set on the <see cref="FocusOnClickProperty"/>.</param>
        public static void SetFocusOnClick(DependencyObject obj, bool value)
        {
            obj.SetValue(FocusOnClickProperty, value);
        }

        /// <summary>
        /// A <see cref="DependencyProperty"/> used to store a registered parent element of a <see cref="Button"/>. This enables the parent's properties to be binded to the property values of the child.
        /// </summary>
        public static readonly DependencyProperty RegisteredParentProperty =
            DependencyProperty.RegisterAttached("RegisteredParent", typeof(FrameworkElement), typeof(ButtonBehaviors), new UIPropertyMetadata(null, new PropertyChangedCallback(OnRegisteredParentPropertySet)));

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
        /// It sets the <see cref="Button"/> with the <see cref="RegisteredParentProperty"/> as a parent element's 
        /// registered child that will be taken into consideration when configuring the parent's properties.
        /// </summary>
        /// <param name="depObj">The <see cref="DependencyObject"/> on which the <see cref="RegisteredParentProperty"/> has changed.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> for <see cref="RegisteredParentProperty"/> changed events.</param>
        private static void OnRegisteredParentPropertySet(DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            if (!(depObj is Button button) || !(GetRegisteredParent(depObj) is FrameworkElement parentElement))
                return;

            if (button.IsLoaded)
            {
                FrameworkElementBehaviors.SetRegisteredChildren(parentElement, button);
            }
            else
            {
                button.Loaded += (sender, eventArgs) => FrameworkElementBehaviors.SetRegisteredChildren(parentElement, button);
            }
        }

        /// <summary>
        /// A <see cref="PropertyChangedCallback"/> that handles <see cref="FocusOnClickProperty"/> changes.
        /// It subscribes the <see cref="RoutedEventHandler"/> <see cref="ButtonBehaviors_Click(object, RoutedEventArgs)"/> to the <see cref="Button"/>'s click event.
        /// </summary>
        /// <param name="sender">The <see cref="DependencyObject"/> on which the <see cref="FocusOnClickProperty"/> has changed.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> for <see cref="FocusOnClickProperty"/> changed events.</param>
        private static void OnFocusOnClickPropertySet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as Button).Click += new RoutedEventHandler(ButtonBehaviors_Click);
        }

        /// <summary>
        /// Focuses on the clicked <see cref="Button"/>.
        /// </summary>
        /// <param name="sender">The <see cref="object"/> that was clicked.</param>
        /// <param name="e">The click event <see cref="RoutedEventArgs"/>.</param>
        static void ButtonBehaviors_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).Focus();
        }
    }
}
