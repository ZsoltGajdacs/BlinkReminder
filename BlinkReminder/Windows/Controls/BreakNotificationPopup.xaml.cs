﻿using BlinkReminder.Settings;
using BlinkReminder.Windows.Support;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BlinkReminder.Windows.Controls
{
    /// <summary>
    /// Interaction logic for BreakNotificationPopup.xaml
    /// </summary>
    public partial class BreakNotificationPopup : UserControl, INotifyPropertyChanged
    {
        private string _textToShow = String.Empty;
        private CountdownTimer countdownTimer;
        private UserSettings settings;

        public bool ShouldStartBreak { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public BreakNotificationPopup(string textToShow)
        {
            InitializeComponent();

            settings = UserSettings.Instance;
            countdownTimer = new CountdownTimer(settings.PreNotificationTime);
            timerBlock.DataContext = countdownTimer;
            controlGrid.DataContext = this;
            TextToShow = textToShow;
            SetBtnLabel();
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

        /// <summary>
        /// Set's the confiramtion button's label according to the chosen mode
        /// </summary>
        private void SetBtnLabel()
        {
            string btnLabel = String.Empty;
            if (settings.IsPermissiveNotification)
            {
                btnLabel = "Let's have a break!";
            }
            else
            {
                btnLabel = "No break yet, I'm not ready!";
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

            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}
