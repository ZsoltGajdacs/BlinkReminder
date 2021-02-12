namespace BlinkReminder.Helpers.ScreenInfo
{
    internal static class ScreenSizeInfo
    {
        /// <summary>
        /// Gives back an array with the screen's resolution (X, Y)
        /// </summary>
        internal static double CalculateLeftEdgeOfWindow(double width)
        {
            System.Windows.Rect desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            return desktopWorkingArea.Right - width;
        }

        internal static double CalculateTopEdgeOfWindow(double height)
        {
            System.Windows.Rect desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            return desktopWorkingArea.Bottom - height;
        }
    }
}
