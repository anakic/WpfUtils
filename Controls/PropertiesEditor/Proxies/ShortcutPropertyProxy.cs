using System.Windows.Input;

namespace Thingie.WPF.Controls.PropertiesEditor.Proxies
{
	//options for how the UI will treat the stroke
	public enum ShortcutAction
    {
        Accept,
        DeleteContent,
        Reject,
        Default
    }

    //interface for custom processing of shortcut strokes
    //tells the UI how to treat strokes(i.e. is a Del key a shortcut, or does it delete the current shortcut value)
    public interface IShortcutRules
    {
        ShortcutAction Process(Key key, ModifierKeys modifierKey);
    }

    //default implementation of IShortcutRules
    public class DefaultShortcutRules : IShortcutRules
    {
        public virtual ShortcutAction Process(Key key, ModifierKeys modifierKey)
        {
            if (modifierKey != ModifierKeys.None)
            {
				if (key == Key.Tab && (modifierKey == ModifierKeys.Windows || modifierKey == ModifierKeys.Alt))
                    return ShortcutAction.Default;
                else if (key != Key.LWin && key != Key.RWin && key != Key.System && key != Key.LeftAlt && key != Key.LeftShift && key != Key.LeftCtrl && key != Key.RightAlt && key != Key.RightShift && key != Key.RightCtrl)
                    return ShortcutAction.Accept;
                else
                    return ShortcutAction.Reject;
            }
            else
            {
                if (key == Key.Delete || key == Key.Back)
                    return ShortcutAction.DeleteContent;
                else if (key == Key.Enter || key == Key.Tab)
                    return ShortcutAction.Default;
                else if ((int)key >= (int)Key.F1 && (int)key <= (int)Key.F24 || key == Key.Pause || key == Key.Escape || key == Key.PrintScreen || key == Key.OemPeriod || key == Key.Space || key==Key.OemComma)
                    return ShortcutAction.Accept;
                else
                    return ShortcutAction.Reject;
            }
        }
    }

    public class ShortcutPropertyProxy : TextPropertyProxy
    {
        public IShortcutRules Rules { get; private set; }

        public ShortcutPropertyProxy()
            : this(new DefaultShortcutRules())
        {
        }

        public ShortcutPropertyProxy(IShortcutRules rules)
        {
            Rules = rules;
        }
    }
}
