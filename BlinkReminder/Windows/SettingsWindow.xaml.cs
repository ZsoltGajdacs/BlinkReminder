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
    /// Code behind for the settings window
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private UserSettings settings;

        public SettingsWindow(ref UserSettings settings)
        {
            InitializeComponent();
            this.settings = settings;

            SetDataBinding();
        }

        private void SetDataBinding()
        {
            SettingsGrid.DataContext = settings;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
