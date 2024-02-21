using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace CyberIDMKing
{
    internal static class WindowManager
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public static IntPtr FindWindowByTitle(string title)
        {
            return FindWindow(null, title);
        }

        //Close a window
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        public static bool CloseWindowByHandle(IntPtr handle)
        {
            return SendMessage(handle, (uint)0x111, IntPtr.Zero, IntPtr.Zero) != IntPtr.Zero;
        }

        //Get window elements
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public static bool BringWindowToForeground(IntPtr handle)
        {
            // Check if the handle is valid
            if (handle == IntPtr.Zero)
            {
                throw new ArgumentException("Invalid window handle");
            }

            // Attempt to bring the window to the foreground
            bool success = SetForegroundWindow(handle);

            // If unsuccessful, check if attaching the thread is necessary
            if (!success)
            {
                // Get the current thread ID
                uint currentThreadId = GetCurrentThreadId();

                // Get the thread ID of the window's process
                uint windowThreadId = GetWindowThreadProcessId(handle, out _);

                // Attach the current thread to the window's thread if different
                if (currentThreadId != windowThreadId)
                {
                    if (AttachThreadInput(currentThreadId, windowThreadId, true))
                    {
                        success = SetForegroundWindow(handle);
                        DetachThreadInput(currentThreadId, windowThreadId);
                    }
                }
            }

            return success;
        }

        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DetachThreadInput(uint idAttach, uint idAttachTo);


    }
}
