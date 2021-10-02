using BRWPF.Utils;
using BRCore.Settings;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace BRWPF.Windows
{
    /// <summary>
    /// Interaction logic for PauseWindow.xaml
    /// </summary>
    public partial class PauseWindow : Window, INotifyPropertyChanged
    {
        private int _pauseTime;
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
        public int PauseTime
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

        public PauseWindow()
        {
            InitializeComponent();
            tooltipHandler = new TooltipHandler();

            IndefPauseBtn.IsEnabled = UserSettings.Instance.IndefPauseEnabled;
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
        public new int ShowDialog()
        {
            base.ShowDialog();

            if (btnClicked)
            {
                PauseTime = pauseTimeControl.Value;
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
