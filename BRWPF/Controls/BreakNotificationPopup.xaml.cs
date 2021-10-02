using BRCore.Settings;
using BRCore.TimerBasedMeasurement;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace BRWPF.Controls
{
    /// <summary>
    /// Interaction logic for BreakNotificationPopup.xaml
    /// </summary>
    public partial class BreakNotificationPopup : UserControl, INotifyPropertyChanged
    {
        private static readonly string GO_BREAK_LABEL = "Let's have a break!";
        private static readonly string NO_BREAK_LABEL = "Can't rest now!";

        private string _textToShow = String.Empty;
        private CountdownTimer countdownTimer;
        private UserSettings settings;

        public bool ShouldStartBreak { get; set; }
        public bool ShouldPostponeBreak { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public BreakNotificationPopup()
        {
            InitializeComponent();
        }

        #region Property changed handler
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Accessors
        public string TextToShow
        {
            get
            {
                return _textToShow;
            }

            set
            {
                if (!value.Equals(_textToShow))
                {
                    _textToShow = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        internal void SetValues(string textToShow, int postponeCount)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                countdownTimer = new CountdownTimer(settings.NotificationLength);
                timerBlock.DataContext = countdownTimer;

                TextToShow = textToShow;
                SetBtnLabel();
                ShouldPostponeBreak = false;

                // Set the default if there is no click
                if (settings.IsPermissiveNotification)
                {
                    ShouldStartBreak = false;
                }
                else
                {
                    ShouldStartBreak = true;
                }

                // If the user already exceeded the set postpone count then it should be disabled
                if (postponeCount >= settings.PostponeAmount)
                {
                    postponeBtn.IsEnabled = false;
                }
                else
                {
                    postponeBtn.IsEnabled = true;
                }
            }));
        }

        /// <summary>
        /// Set's the confirmation button's label according to the chosen mode
        /// </summary>
        private void SetBtnLabel()
        {
            string btnLabel = String.Empty;
            if (settings.IsPermissiveNotification)
            {
                btnLabel = GO_BREAK_LABEL;
            }
            else
            {
                btnLabel = NO_BREAK_LABEL;
            }

            confirmBtn.Content = btnLabel;
        }

        private void ConfirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (settings.IsPermissiveNotification)
            {
                ShouldStartBreak = true;
            }
            else
            {
                ShouldStartBreak = false;
            }

            CloseThisBallon();
        }

        private void PostponeBtn_Click(object sender, RoutedEventArgs e)
        {
            ShouldPostponeBreak = true;
            CloseThisBallon();
        }

        private void CloseThisBallon()
        {
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}
