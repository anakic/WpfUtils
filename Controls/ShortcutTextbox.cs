using Thingie.WPF.Controls.PropertiesEditor.Proxies;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Thingie.WPF.Controls
{
	class ShortcutTextbox : TextBox
    {
        public static readonly DependencyProperty RulesProperty = DependencyProperty.Register("Rules", typeof(IShortcutRules), typeof(ShortcutTextbox), new PropertyMetadata(new DefaultShortcutRules()));
        public IShortcutRules Rules
        {
            get { return (IShortcutRules)GetValue(RulesProperty); }
            set { SetValue(RulesProperty, value); }
        }

        bool _accept;
        string _backup;

        protected override void OnGotFocus(System.Windows.RoutedEventArgs e)
        {
            _accept = true;//ako se sa Tab dodje ovdje, da ne zezne OnPreviewKeyUp
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (e.IsRepeat)
                return;

            _accept = false;
            _backup = this.Text;

            var action = Rules.Process(e.Key, Keyboard.Modifiers);
            switch (action)
            {
                case ShortcutAction.Accept:
                    if (Keyboard.Modifiers == ModifierKeys.None)
                        this.Text = e.Key.ToString();
                    else
                        this.Text = string.Format("{0}+{1}", Keyboard.Modifiers, e.Key);
                    e.Handled = true;
                    _accept = true;
                    break;
                case ShortcutAction.DeleteContent:
                    this.Text = string.Empty;
                    e.Handled = true;
                    _accept = true;
                    break;
                case ShortcutAction.Reject:
                    e.Handled = true;
                    _accept = false;
                    break;
                case ShortcutAction.Default:
                    e.Handled = false;
                    _accept = true;
                    break;
                default:
                    throw new ArgumentException("Invalid ShortcutAction!");
            }
        }

        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            if (!_accept)
            {
                e.Handled = true;
                Text = _backup;
            }
        }
    }
}
