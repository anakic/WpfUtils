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
    public abstract class ShortcutHandle : MarshalByRefObject
    {
        // todo: memory leak? when should we call RemotingServices.Disconnect(this)?
        public override object InitializeLifetimeService() => null;

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
        internal ShortcutGesture KeyGesture { get { return new ShortcutGesture(Key, ModifierKeys); } set { Key = value.Key; ModifierKeys = value.ModifierKeys; } }
    }

    [ProxyFactory(typeof(ShortcutServiceProxyFactory))]
    public class ShortcutService : MarshalByRefObject
    {
        // todo: memory leak? when should we call RemotingServices.Disconnect(this)?
        public override object InitializeLifetimeService() => null;

        public List<ShortcutHandle> Shortcuts { get; set; }

        public ShortcutService()
            : this((uint)AppDomain.GetCurrentThreadId())
        {
        }

        public ShortcutService(uint mainThreadId)
        {
            Shortcuts = new List<ShortcutHandle>();
            _proc = HookCallback;
            _hookID = SetWindowsHookEx(WH_KEYBOARD, _proc, IntPtr.Zero, mainThreadId);
        }

        public void AddShortcut(ShortcutHandle handle)
        {
            this.Shortcuts.Add(handle);
        }

        public void RemoveShortcut(ShortcutHandle handle)
        {
            this.Shortcuts.Remove(handle);
        }

        #region winapi stuff
        public delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private readonly LowLevelKeyboardProc _proc;
        private readonly IntPtr _hookID = IntPtr.Zero;

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

		private ShortcutHandle FindShortcut()
		{
			return Shortcuts.FirstOrDefault(sh =>
				sh.KeyGesture.Key != Key.None &&
				Keyboard.IsKeyDown(sh.KeyGesture.Key) &&
				Keyboard.Modifiers == sh.KeyGesture.ModifierKeys &&
				sh.IsInScope());
		}

		protected virtual void BeforeExecuteShortcut(ShortcutHandle shortcutHandle) { }

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
