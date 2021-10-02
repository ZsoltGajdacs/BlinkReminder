using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BRCore.Utils
{
    /// <summary>
    /// Static class for locking the workstation
    /// </summary>
    public static class WorkStationLock
    {
        /// <summary>
        /// Locks the workstation
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool LockWorkStation();
    }
}
