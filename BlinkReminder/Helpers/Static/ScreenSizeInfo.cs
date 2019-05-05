using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlinkReminder.Helpers
{
    internal static class ScreenSizeInfo
    {
        /// <summary>
        /// Gives back an array with the screen's resolution (X, Y)
        /// </summary>
        internal static double CalculateLeftEdgeOfWindow(double width)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            return desktopWorkingArea.Right - width;
        }

        internal static double CalculateTopEdgeOfWindow(double height)
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            return desktopWorkingArea.Bottom - height;
        }
    }
}
