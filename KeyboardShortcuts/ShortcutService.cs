using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;
using Thingie.WPF.Attributes;

namespace Thingie.WPF.KeyboardShortcuts
{
    public abstract class ShortcutHandle
    {
        public string Name { get; set; }
        public string Category { get; set; }

        public Key Key { get; set; }
        public ModifierKeys ModifierKeys { get; set; }

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

        public abstract bool CanExecute();
        public abstract bool IsInScope();
        public abstract void Execute();

        //sluzi samo za komunikaciju sa property gridom (da objedini dva property-a u jedan, tako da key i modifier mogu editirati u istom retku u gridu)
        public ShortcutGesture KeyGesture { get { return new ShortcutGesture(Key, ModifierKeys); } set { Key = value.Key; ModifierKeys = value.ModifierKeys; } }
    }

    [ProxyFactory(typeof(ShortcutServiceProxyFactory))]
    public class ShortcutService : IDisposable
    {
        public List<ShortcutHandle> Shortcuts { get; set; }

        int threadId;
        public ShortcutService()
        {
            threadId = Thread.CurrentThread.ManagedThreadId;

            Shortcuts = new List<ShortcutHandle>();
            proc = HookCallback;
#pragma warning disable CS0618 // Type or member is obsolete
            hook = NativeMethods.SetWindowsHookEx(WH_KEYBOARD, proc, IntPtr.Zero, (uint)AppDomain.GetCurrentThreadId());
#pragma warning restore CS0618 // Type or member is obsolete
        }

        /// <summary>
        /// Global hook for another process
        /// </summary>
        /// <param name="p"></param>
        public ShortcutService(Process p)
        {
            threadId = Thread.CurrentThread.ManagedThreadId;

            Shortcuts = new List<ShortcutHandle>();
            proc = HookCallback;
            hook = NativeMethods.SetWindowsHookEx(WH_KEYBOARD_LL, proc, NativeMethods.GetModuleHandle(p.MainModule.ModuleName), 0);
        }

        public void AddShortcut(ShortcutHandle handle)
        {
            this.Shortcuts.Add(handle);
        }

        public void RemoveShortcut(ShortcutHandle handle)
        {
            this.Shortcuts.Remove(handle);
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                // When pressing Enter to select an item in the windows clipboard history
                // the following combination is true: lParam == 1, Ctrl+Enter is detected as pressed.
                // We want to skip handling this, it's not for us. We don't want to prevent the
                // clipboard history item to be activated. The clipboard is not a window it's an "overlay"
                // (if ChatGPT is to be believed) so it does not have a different hwnd, so we must
                // handle this situation here.
                if (nCode == HC_ACTION && (lParam != new IntPtr(0x1) || !Keyboard.IsKeyDown(Key.Enter) || Keyboard.Modifiers != ModifierKeys.Control))
                {
                    ShortcutHandle shortcutHandle = FindShortcut();

                    if (shortcutHandle != null)
                    {
                        if (shortcutHandle.CanExecute())
                        {
                            BeforeExecuteShortcut(shortcutHandle);
                            shortcutHandle.Execute();
                        }
                        return (IntPtr)(int)-1;
                    }
                    else
                        return NativeMethods.CallNextHookEx(hook.Handle, nCode, wParam, lParam);
                }
                else
                    return NativeMethods.CallNextHookEx(hook.Handle, nCode, wParam, lParam);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
                return NativeMethods.CallNextHookEx(hook.Handle, nCode, wParam, lParam);
            }
        }

        private ShortcutHandle FindShortcut()
        {
            return Shortcuts.FirstOrDefault(sh =>
                sh.KeyGesture.Key != Key.None &&
                Keyboard.IsKeyDown(sh.KeyGesture.Key) &&
                Keyboard.Modifiers == sh.KeyGesture.ModifierKeys &&
                sh.IsInScope());
        }

        protected virtual void BeforeExecuteShortcut(ShortcutHandle shortcutHandle) { }

        #region winapi stuff
        private NativeMethods.LowLevelKeyboardProc proc;
        private SetWinEventHookExHandle hook;
        private bool disposedValue;

        //Declare the mouse hook constant.
        //For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.            
        private const int WH_KEYBOARD_LL = 13;
        private const int WH_KEYBOARD = 2;
        private const int HC_ACTION = 0;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                // doesn't hurt to try, even if on wrong thread
                hook.Dispose();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
