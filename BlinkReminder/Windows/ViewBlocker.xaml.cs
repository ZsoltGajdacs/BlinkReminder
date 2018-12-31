﻿using System;
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
    public partial class ViewBlocker : Window, IDisposable
    {
        private Timer timeOfBlock;

        public ViewBlocker(long interval, bool isSkippable, string message)
        {
            InitializeComponent();
            StartDisplayTimer(interval);
            SetSkippable(isSkippable);
            SetMessage(message);
            PrepareWindow();
        }

        private void SetMessage(string msg)
        {
            longText.Text = msg;
        }

        private void SetSkippable(bool isSkippable)
        {
            if (!isSkippable)
            {
                skipButton.IsEnabled = false;
                skipButton.Visibility = Visibility.Hidden;
            }
        }

        private void PrepareWindow()
        {
            Application.Current.MainWindow = this; // Set to mainwindow
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                Application.Current.MainWindow.Activate();
                Application.Current.MainWindow.Topmost = true;
                Application.Current.MainWindow.Focus();
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    timeOfBlock.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ViewBlocker() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
