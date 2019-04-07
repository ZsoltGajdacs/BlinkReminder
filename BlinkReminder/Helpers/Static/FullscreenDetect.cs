/*
 * From Stackoverflow user ARN: https://stackoverflow.com/users/7967049/arn
 * Taken from this answer: https://stackoverflow.com/a/43797777
 * Modified to fit my needs - Zsolt Gajdács
*/
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;

namespace BlinkReminder.Helpers
{
    /// <summary>
    /// Contains methods for detecting fullscreen applications. 
    /// Named NativeMethods 'cause VS2017 said so :D
    /// </summary>
    internal static class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(HandleRef hWnd, [In, Out] ref RECT rect);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// Gives back the process for the given handler
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        private static Process GetProcessOfWindow(IntPtr hWnd)
        {
            uint procId = 0;
            GetWindowThreadProcessId(hWnd, out procId);

            return Process.GetProcessById((int)procId);
        }

        /// <summary>
        /// Checks if there is a fullscreen process running. 
        /// Gives back the process for the foreground window.
        /// </summary>
        /// <param name="windowProcess"></param>
        /// <param name="screen"></param>
        /// <returns></returns>
        public static bool IsFullscreenAppRunning(out Process windowProcess, Screen screen = null)
        {
            if (screen == null)
            {
                screen = Screen.PrimaryScreen;
            }
            RECT rect = new RECT();
            IntPtr hWnd = (IntPtr)GetForegroundWindow();

            GetWindowRect(new HandleRef(null, hWnd), ref rect);

            windowProcess = GetProcessOfWindow(hWnd);

            if (screen.Bounds.Width == (rect.right - rect.left) && screen.Bounds.Height == (rect.bottom - rect.top))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
