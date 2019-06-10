using BlinkReminder.Windows.Support;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public BreakNotificationPopup(string textToShow)
        {
            InitializeComponent();

            countdownTimer = new CountdownTimer(TimeSpan.FromSeconds(10));
            timerBlock.DataContext = countdownTimer;
            controlGrid.DataContext = this;
            TextToShow = textToShow;
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
    }
}
