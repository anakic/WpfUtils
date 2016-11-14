using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using Thingie.WPF.Attributes;

namespace Thingie.WPF.KeyboardShortcuts
{
    public class ShortcutHandle
    {
        public string Name { get; set; }
        public string Category { get; set; }

        public Key Key { get; set; }
        public ModifierKeys ModifierKeys { get; set; }
        public FrameworkElement Scope { get; private set; }

        public Func<bool> CanExecute { get; private set; }
        public Action Execute { get; private set; }

        public ShortcutHandle(string name, Key key)
            : this(name, key, ModifierKeys.None)
        { }
        public ShortcutHandle(string name, Key key, ModifierKeys modifierKeys)
        {
            Key = key;
            ModifierKeys = modifierKeys;
            Name = name;
            this.Category = "General shortcuts";
        }

        public void Claim(Action execute, FrameworkElement scope)
        {
            Claim(execute, null, scope);
        }

        public void Claim(Action execute, Func<bool> canExecute, FrameworkElement scope)
        {
            Scope = scope;
            CanExecute = canExecute;
            Execute = execute;
        }

        //sluzi samo za komunikaciju sa property gridom (da objedini dva property-a u jedan, tako da key i modifier mogu editirati u istom retku u gridu)
        internal ShortcutGesture KeyGesture { get { return new ShortcutGesture(Key, ModifierKeys); } set { Key = value.Key; ModifierKeys = value.ModifierKeys; } }

    }

    [ProxyFactory(typeof(ShortcutServiceProxyFactory))]
    public class ShortcutService
    {
        public List<ShortcutHandle> Shortcuts { get; set; }

        public ShortcutService()
        {
            Shortcuts = new List<ShortcutHandle>();
            _proc = HookCallback;
            _hookID = SetWindowsHookEx(WH_KEYBOARD, _proc, IntPtr.Zero, (uint)AppDomain.GetCurrentThreadId());
        }

        #region winapi stuff
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;

        //Declare the mouse hook constant.
        //For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.            
        private const int WH_KEYBOARD = 2;
        private const int HC_ACTION = 0;

        public void ReleaseHook()
        {
            UnhookWindowsHookEx(_hookID);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                if (nCode == HC_ACTION)
                {
                    var shortcutHandle = Shortcuts.FirstOrDefault(sh =>
                        sh.Execute != null &&
                        sh.KeyGesture.Key != Key.None &&
                        Keyboard.IsKeyDown(sh.KeyGesture.Key) &&
                        Keyboard.Modifiers == sh.KeyGesture.ModifierKeys &&
                        (sh.Scope == null || IsInScope((DependencyObject)Keyboard.FocusedElement, sh.Scope)));

                    if (shortcutHandle != null)
                    {
                        if (shortcutHandle.CanExecute == null || shortcutHandle.CanExecute())
                            shortcutHandle.Execute();
                        return (IntPtr)(int)-1;
                    }
                    else
                        return CallNextHookEx(_hookID, nCode, wParam, lParam);
                }
                else
                    return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return CallNextHookEx(_hookID, nCode, wParam, lParam);
            }
        }

        private static bool IsInScope(DependencyObject element, DependencyObject scope)
        {
            if (scope == null)
                return true;
            else
            {
                if (element == null)
                    return false;
                else if (element == scope)
                    return true;
                else
                    return IsInScope(VisualTreeHelper.GetParent(element), scope);
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        ~ShortcutService()
        {
            ReleaseHook();
        }

        #endregion
    }
}
