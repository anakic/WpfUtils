using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Thingie.WPF
{
    //typeconverter for shortcutkey
    internal class KeyboardShortcutTypeConverter : StringConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (!(value is string))
                throw new ArgumentException();
            else
            {
                ModifierKeys modif = ModifierKeys.None;
                Key key = Key.None;

                string str = value as string;
                
                Match m = Regex.Match(str, @"((?'modif'.+)\s*\+)?\s*(?'key'.*)");
                if (m.Groups["modif"].Success)
                {
                    int val = 0;
                    foreach (string modifSegment in m.Groups["modif"].Value.Split(','))
                    {
                        val |= (int)TypeDescriptor.GetConverter(typeof(ModifierKeys)).ConvertFromString(modifSegment.Trim());
                    }
                    modif = (ModifierKeys)val;
                }
                if (m.Groups["key"].Success)
                    key = (Key)TypeDescriptor.GetConverter(typeof(Key)).ConvertFromString(m.Groups["key"].Value);

                return new ShortcutGesture(key, modif);
            }
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (!(value is ShortcutGesture) || destinationType != typeof(string))
                throw new ArgumentException();
            else
            {
                ShortcutGesture shortcut = (ShortcutGesture)value;
                if (shortcut.ModifierKeys == ModifierKeys.None)
                    return shortcut.Key.ToString();
                else
                    return string.Format("{0}+{1}", shortcut.ModifierKeys, shortcut.Key);
            }
        }
    }

    [TypeConverter(typeof(KeyboardShortcutTypeConverter)), Serializable]
    public struct ShortcutGesture
    {
        public Key Key;
        public ModifierKeys ModifierKeys;

        public ShortcutGesture(Key key) : this(key, ModifierKeys.None)
        { }
        public ShortcutGesture(Key key, ModifierKeys modifierKeys)
        {
            Key = key;
            ModifierKeys = modifierKeys;
        }
    }
}
