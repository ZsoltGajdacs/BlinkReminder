using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BlinkReminder.Helpers
{
    internal class TooltipHandler
    {
        private ToolTip toolTip;
        private TextBox tBWithOpenTT; // This one will keep the TB with an open Tooltip

        internal TooltipHandler()
        {
            toolTip = new ToolTip();
            toolTip.Closed += Tt_Closed;
            toolTip.StaysOpen = false;
        }

        /// <summary>
        /// Shows a Tooltip with the given message on the given Textbox
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="msg"></param>
        internal void ShowTooltipOnTextBox(ref TextBox textBox, string msg)
        {
            // Get the reference address of the tooltip so the tooltip can be removed later
            tBWithOpenTT = textBox;

            // Show tooltip
            Application.Current.Dispatcher.Invoke(() =>
            {
                tBWithOpenTT.ToolTip = toolTip;
                toolTip.Content = msg;
                toolTip.IsOpen = true;
            });
        }

        /// <summary>
        /// Runs when the tooltip of one of the input controls is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Tt_Closed(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                toolTip.IsOpen = false;
                tBWithOpenTT.ToolTip = null;
            });
        }
    }
}
