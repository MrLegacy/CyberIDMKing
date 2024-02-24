using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CyberIDMKing
{
    internal static class WindowManager
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        public static IntPtr FindWindowByTitle(string title,string className=null)
        {
            return FindWindow(className, title);
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

        //Get window Process PID
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(HandleRef handle, out int processId);

        public static int GetProcessPID(IntPtr handle)
        {
            GetWindowThreadProcessId(handle,out var processId);
            return (int)processId;
        }

        //====================================
        private delegate bool EnumWindowProc(IntPtr hwnd, IntPtr lParam);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr lParam);

        

        public static List<IntPtr> GetAllChildHandles(IntPtr handle)
        {
            List<IntPtr> childHandles = new List<IntPtr>();

            GCHandle gcChildhandlesList = GCHandle.Alloc(childHandles);
            IntPtr pointerChildHandlesList = GCHandle.ToIntPtr(gcChildhandlesList);

            try
            {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(handle, childProc, pointerChildHandlesList);
            }
            finally
            {
                gcChildhandlesList.Free();
            }

            return childHandles;
        }

        private static bool EnumWindow(IntPtr hWnd, IntPtr lParam)
        {
            GCHandle gcChildhandlesList = GCHandle.FromIntPtr(lParam);

            if (gcChildhandlesList == null || gcChildhandlesList.Target == null)
            {
                return false;
            }

            List<IntPtr> childHandles = gcChildhandlesList.Target as List<IntPtr>;
            childHandles.Add(hWnd);

            return true;
        }

        //Get class name
        public static string Dialog_Class = "#32770";
        public static string Button_Class = "Button";
        public static string Static_Class = "Static";

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        public static string GetClassName(IntPtr hWnd)
        {
            StringBuilder className = new StringBuilder(256);
            GetClassName(hWnd, className, className.Capacity);
            return className.ToString();
        }

        //=================
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        public static string GetWindowTitle(IntPtr hWnd)
        {
            var length = GetWindowTextLength(hWnd) + 1;
            var title = new StringBuilder(length);
            GetWindowText(hWnd, title, length);
            return title.ToString();
        }
        //======================
        public static IntPtr FindWindow(string title, string childCaption, string className,int pid)
        {
            var handle = FindWindowByTitle(title, className);
            if (handle != IntPtr.Zero)
            {
                if (pid == GetProcessPID(handle))
                {
                    var handles = GetAllChildHandles(handle);
                    foreach (var h in handles)
                    {
                        var childTitle = GetWindowTitle(h);
                        //var childClassName = WindowManager.GetClassName(h);
                        if (childTitle.Contains(childCaption))
                            return handle;
                    }
                }
            }
            return IntPtr.Zero;
        }
        //===================
        public static void BringAndClose(IntPtr handle,int pid)
        {
            try
            {
                if (handle != IntPtr.Zero)
                {
                    if (pid == GetProcessPID(handle))
                        if (BringWindowToForeground(handle))
                        {
                            SendKeys.Send("{ESC}");
                            if (CloseWindowByHandle(handle))
                            {
                                //send esc to the window

                            }
                        }
                        if (BringWindowToForeground(handle))
                        {
                            SendKeys.Send("{TAB}");
                            SendKeys.Send("{ENTER}");
                        }
                }
            }catch { }
        }
        //=========================
        delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);

        public static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id,
                    (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);

            return handles;
        }
    }
}
