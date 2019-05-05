using BlinkReminder.Helpers;
using BlinkReminder.Settings;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Xceed.Wpf.Toolkit;

namespace BlinkReminder.Windows
{
    /// <summary>
    /// Interaction logic for PauseWindow.xaml
    /// </summary>
    public partial class PauseWindow : Window
    {
        private UserSettings userSettings;
        public long PauseTime { get; set; }
        private bool btnClicked;
        private TooltipHandler tooltipHandler;

        internal PauseWindow()
        {
            InitializeComponent();
            userSettings = UserSettings.Instance;
            tooltipHandler = new TooltipHandler();

            IndefPauseBtn.IsEnabled = userSettings.IndefPauseEnabled;
            PauseTime = 1;

            SetBinding();
        }

        /// <summary>
        /// Sets the binding for the main stack
        /// </summary>
        private void SetBinding()
        {
            mainStack.DataContext = this;
        }

        /// <summary>
        /// Shows the window and returns with the time selection the user chose. 
        /// -1 if indefinite
        /// -2 if user closed window with 'x'
        /// </summary>
        /// <returns></returns>
        public new long ShowDialog()
        {
            base.ShowDialog();

            if (btnClicked)
            {
                return PauseTime;
            }
            else
            {
                return -2;
            }
        }

        #region Button Clicks
        private void IndefPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            PauseTime = -1;
            btnClicked = true;
            Close();
        }

        private void TimedPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            btnClicked = true;
            Close();
        }

        #endregion
    }
}
