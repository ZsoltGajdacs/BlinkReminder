using BlinkReminder.Helpers;
using BlinkReminder.Settings;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BlinkReminder.Windows
{
    /// <summary>
    /// Interaction logic for PauseWindow.xaml
    /// </summary>
    public partial class PauseWindow : Window, INotifyPropertyChanged
    {
        private UserSettings userSettings;
        private long _pauseTime;
        private bool btnClicked;
        private TooltipHandler tooltipHandler;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Property changed handler
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
        public long PauseTime
        {
            get
            {
                return _pauseTime;
            }

            set
            {
                if (value != _pauseTime)
                {
                    _pauseTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

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
                PauseTime = (long)pauseTimeControl.Value;
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
