using System.Windows;
using System.Windows.Controls;

namespace BRWPF.Utils
{
    internal class TooltipHandler
    {
        private ToolTip toolTip;
        private Control uiControl; // This one will keep the TB with an open Tooltip

        internal TooltipHandler()
        {
            toolTip = new ToolTip();
            toolTip.Closed += Tt_Closed;
            toolTip.StaysOpen = false;
        }

        /// <summary>
        /// Shows a Tooltip with the given message on the given Control
        /// </summary>
        internal void ShowTooltipOnTextBox(ref Control control, string msg)
        {
            // Get the reference address of the tooltip so the tooltip can be removed later
            uiControl = control;

            // Show tooltip
            Application.Current.Dispatcher.Invoke(() =>
            {
                uiControl.ToolTip = toolTip;
                toolTip.Content = msg;
                toolTip.IsOpen = true;
            });
        }

        /// <summary>
        /// Runs when the tooltip of one of the input controls is closed
        /// </summary>
        private void Tt_Closed(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                toolTip.IsOpen = false;
                uiControl.ToolTip = null;
            });
        }
    }
}
