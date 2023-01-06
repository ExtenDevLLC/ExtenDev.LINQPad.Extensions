using System;
using System.Diagnostics;
using System.Windows.Forms;
using LINQPad;

namespace ExtenDev.LINQPad.Extensions.UI
{
    /// <summary>
    /// Provides a managed wrapper that allows the LINQPad Host Process' Main Window to be used as the parent for modal dialogs.
    /// </summary>
    public class LINQPadOwnerWindow : IWin32Window
    {
        private LINQPadOwnerWindow(IntPtr hwnd)
        {
            Handle = hwnd;
        }

        /// <inheritdoc cref="IWin32Window.Handle"/>
        public IntPtr Handle { get; }

        private static LINQPadOwnerWindow instance = new LINQPadOwnerWindow(Process.GetProcessById(Util.HostProcessID).MainWindowHandle);

        /// <summary>
        /// Gets the current LINQPad Host Process Main Window for use with parenting modal dialogs.
        /// </summary>
        /// <remarks>
        /// Returns <see langword="null"/> when the debugger is attached in order to prevent UI deadlocks.
        /// </remarks>
        public static LINQPadOwnerWindow? Instance
        {
            get
            {
                return Debugger.IsAttached ? null : instance;
            }
        }
    }
}
