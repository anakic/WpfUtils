using Microsoft.Win32.SafeHandles;
using System;
using System.Diagnostics;
using System.Runtime.ConstrainedExecution;
using System.Security.Permissions;
using System.Threading;

namespace Thingie.WPF.KeyboardShortcuts
{
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    public class SetWinEventHookExHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        int ThreadId;

        public IntPtr Handle => handle;

        // Create a SafeHandle, informing the base class
        // that this SafeHandle instance "owns" the handle,
        // and therefore SafeHandle should call
        // our ReleaseHandle method when the SafeHandle
        // is no longer in use.
        private SetWinEventHookExHandle()
            : base(true)
        {
            ThreadId = Thread.CurrentThread.ManagedThreadId;
            if (ThreadId != 1)
                throw new InvalidOperationException();
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        override protected bool ReleaseHandle()
        {
            Debug.Assert(
                ThreadId == Thread.CurrentThread.ManagedThreadId,
                $"Must unhook from win API events on the same thread as when hooking! Hooked on thread {ThreadId}, unhooking on thread {Thread.CurrentThread.ManagedThreadId}.");

            // Here, we must obey all rules for constrained execution regions.
            return NativeMethods.UnhookWindowsHookEx(handle);
            // If ReleaseHandle failed, it can be reported via the
            // "releaseHandleFailed" managed debugging assistant (MDA).  This
            // MDA is disabled by default, but can be enabled in a debugger
            // or during testing to diagnose handle corruption problems.
            // We do not throw an exception because most code could not recover
            // from the problem.
        }
    }
}
