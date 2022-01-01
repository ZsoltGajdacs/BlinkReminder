using System.Runtime.InteropServices;

namespace BRCore.Utils
{
    /// <summary>
    /// Static class for locking the workstation
    /// </summary>
    internal static class WorkStationLock
    {
        /// <summary>
        /// Locks the workstation
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool LockWorkStation();
    }
}
