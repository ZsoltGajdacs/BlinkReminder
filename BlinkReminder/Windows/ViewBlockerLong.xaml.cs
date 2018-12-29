using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
using System.Timers;

namespace BlinkReminder
{
    public partial class ViewBlockerLong : Window, IDisposable
    {
        private Timer timeOfBlock;

        public ViewBlockerLong(long interval)
        {
            InitializeComponent();
            StartDisplayTimer(interval);
            PrepareWindow();
        }

        private void PrepareWindow()
        {
            Application.Current.MainWindow = this; // Set to mainwindow
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Application.Current.MainWindow.Activate();
            }));
        }

        private void StartDisplayTimer(long interval)
        {
            timeOfBlock = new Timer(interval)
            {
                AutoReset = false
            };
            timeOfBlock.Elapsed += TimeOfBlock_Elapsed;
            timeOfBlock.Start();
        }

        private void CloseBlockWindow()
        {
            if (timeOfBlock != null)
            {
                timeOfBlock.Stop();
                timeOfBlock.Dispose();
            }

            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Application.Current.MainWindow.Close();
            }));
        }

        private void TimeOfBlock_Elapsed(object sender, ElapsedEventArgs e)
        {
            CloseBlockWindow();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            CloseBlockWindow();
        }

        public void Dispose()
        {
            ((IDisposable)timeOfBlock).Dispose();
        }
    }
}
