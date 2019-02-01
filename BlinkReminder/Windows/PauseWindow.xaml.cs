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

        public PauseWindow()
        {
            InitializeComponent();
            userSettings = UserSettings.Instance;
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

        private void IndefPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            pauseTime = -1;

            Close();
        }

        private void TimedPauseBtn_Click(object sender, RoutedEventArgs e)
        {
            pauseTime = userSettings.PauseTime;

            Close();
        }

        /// <summary>
        /// Shows the window and returns with the time selection the user chose
        /// </summary>
        /// <returns></returns>
        public new int ShowDialog()
        {
            base.ShowDialog();

            return pauseTime;
        }
    }
}
