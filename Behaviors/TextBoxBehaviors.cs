using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Data;

namespace Thingie.WPF.Behaviors
{
    public class TextBoxBehaviors
    {
        #region auto-move on key
        public static char GetAutoMoveOnKey(DependencyObject obj)
        {
            return (char)obj.GetValue(AutoMoveOnKeyProperty);
        }

        public static void SetAutoMoveOnKey(DependencyObject obj, Key value)
        {
            obj.SetValue(AutoMoveOnKeyProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoMoveOnKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoMoveOnKeyProperty =
            DependencyProperty.RegisterAttached("AutoMoveOnKey", typeof(Key), typeof(TextBoxBehaviors), new UIPropertyMetadata(Key.Tab, new PropertyChangedCallback(OnAutoMoveOnKeySet)));

        private static void OnAutoMoveOnKeySet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as TextBox).AcceptsReturn = true;
            (sender as TextBox).PreviewKeyDown += new KeyEventHandler(TextAutoMoveOnKey_PreviewKeyDown);
        }

        static void TextAutoMoveOnKey_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            Key key = (Key)(sender as TextBox).GetValue(AutoMoveOnKeyProperty);
            if ((e.Key == key)&&(Keyboard.Modifiers==ModifierKeys.None))
            {
                (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
            }
        }
        #endregion

        #region TextMaxLengthProperty
        public static int GetTextMaxLength(DependencyObject obj)
        {
            return (int)obj.GetValue(TextMaxLengthProperty);
        }

        public static void SetTextMaxLength(DependencyObject obj, int value)
        {
            obj.SetValue(TextMaxLengthProperty, value);
        }

        // Using a DependencyProperty as the backing store for TextMaxLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextMaxLengthProperty =
            DependencyProperty.RegisterAttached("TextMaxLength", typeof(int), typeof(TextBoxBehaviors), new UIPropertyMetadata(0, new PropertyChangedCallback(OnTextMaxLengthSet)));

        private static void OnTextMaxLengthSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as TextBox).TextChanged += new TextChangedEventHandler(TextAutoMoveNextBehavior_TextChanged);
        }

        static void TextAutoMoveNextBehavior_TextChanged(object sender, TextChangedEventArgs e)
        {
            if ((sender as TextBox).Text.Length >= (int)((sender as TextBox).GetValue(TextMaxLengthProperty)))
                (sender as TextBox).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
        #endregion

        #region AutoSelectTextOnFocusProperty

        public static bool GetAutoSelectTextOnFocus(DependencyObject obj)
        {
            return (bool)obj.GetValue(AutoSelectTextOnFocusProperty);
        }

        public static void SetAutoSelectTextOnFocus(DependencyObject obj, bool value)
        {
            obj.SetValue(AutoSelectTextOnFocusProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoSelectTextOnFocus.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoSelectTextOnFocusProperty =
            DependencyProperty.RegisterAttached("AutoSelectTextOnFocus", typeof(bool), typeof(TextBoxBehaviors), new UIPropertyMetadata(false, new PropertyChangedCallback(OnAutoSelectTextOfFocusSet)));

        private static void OnAutoSelectTextOfFocusSet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as TextBox).GotFocus += new RoutedEventHandler(TextBoxBehaviors_GotFocus);
            (sender as TextBox).PreviewMouseDown += new MouseButtonEventHandler(TextBoxBehaviors_PreviewMouseDown);
        }

        static void TextBoxBehaviors_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender as TextBox).IsFocused)
            {
                (sender as TextBox).Focus();
                e.Handled = true;
            }
        }

        static void TextBoxBehaviors_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }
        #endregion

        #region cancel key
        
        #endregion

        public static Key GetCancelKey(DependencyObject obj)
        {
            return (Key)obj.GetValue(CancelKeyProperty);
        }

        public static void SetCancelKey(DependencyObject obj, Key value)
        {
            obj.SetValue(CancelKeyProperty, value);
        }

        // Using a DependencyProperty as the backing store for AutoMoveOnKey.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CancelKeyProperty =
            DependencyProperty.RegisterAttached("CancelKeyProperty", typeof(Key), typeof(TextBoxBehaviors), new UIPropertyMetadata(Key.Tab, new PropertyChangedCallback(OnCancelKeySet)));

        private static void OnCancelKeySet(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = (sender as TextBox);

            textBox.PreviewKeyDown += (kdSender, kdArgs) =>
            {
                //kdsender == textBox
                if (kdArgs.Key == (Key)textBox.GetValue(CancelKeyProperty))
                {
                    string textBoxValue = textBox.Text;
                    textBox.GetBindingExpression(TextBox.TextProperty).UpdateTarget();
                    
                    //if anything was canceled, stop the propagation of the keydown event.
                    //This is for the situation where the cancelkey=escape, and
                    //the textbox is in a dialog with a cancel button - 
                    //the cancel button must not fire if anything was canceled.
                    //If nothing was canceled, however, the event is free to propagate.
                    if(textBox.Text!=textBoxValue)
                        kdArgs.Handled = true;
                }
            };
        }
    }
}
