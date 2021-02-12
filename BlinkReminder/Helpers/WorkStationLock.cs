using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BlinkReminder.Helpers
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
        internal static extern bool LockWorkStation();
    }
}
