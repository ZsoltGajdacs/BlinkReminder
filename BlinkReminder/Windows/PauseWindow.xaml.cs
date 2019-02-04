using BlinkReminder.Helpers;
using BlinkReminder.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BlinkReminder.Windows
{
    /// <summary>
    /// Interaction logic for PauseWindow.xaml
    /// </summary>
    public partial class PauseWindow : Window
    {
        private UserSettings userSettings;
        private int pauseTime;
        private bool btnClicked;
        private TooltipHandler tooltipHandler;

        public PauseWindow()
        {
            InitializeComponent();
            userSettings = UserSettings.Instance;
            tooltipHandler = new TooltipHandler();
            IndefPauseBtn.IsEnabled = userSettings.IndefPauseEnabled;

            SetBinding();
        }

        /// <summary>
        /// Sets the binding for the main stack
        /// </summary>
        private void SetBinding()
        {
            mainStack.DataContext = userSettings;
        }

        /// <summary>
        /// Shows the window and returns with the time selection the user chose. 
        /// -1 if indefinite
        /// -2 if user closed window with 'x'
        /// </summary>
        /// <returns></returns>
        public new int ShowDialog()
        {
            base.ShowDialog();

            if (btnClicked)
            {
                return pauseTime;
            }
            else
            {
                return -2;
            }
        }

        #region Event Handlers
        private void IndefPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            pauseTime = -1;
            btnClicked = true;
            Close();
        }

        private void TimedPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            if (userSettings.PauseTime != null)
            {
                pauseTime = (int)userSettings.PauseTime;
                btnClicked = true;
                Close();
            }
            else
            {
                tooltipHandler.ShowTooltipOnTextBox(ref pauseTimeTextBox, "Please enter a value");
            }
        }

        private void PauseTimeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !TextValidator.IsNumsOnly(e.Text);
        }

        private void PauseTimeTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text.Equals("0"))
            {
                textBox.Text = null;
                tooltipHandler.ShowTooltipOnTextBox(ref textBox, "Can't have 0 minutes!");
            }
        }
        #endregion
    }
}
