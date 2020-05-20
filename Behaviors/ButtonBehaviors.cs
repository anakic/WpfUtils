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
