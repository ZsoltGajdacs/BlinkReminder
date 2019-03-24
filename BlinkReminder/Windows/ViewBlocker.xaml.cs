using System;
using System.Windows;
using System.Timers;
using BlinkReminder.Windows.Support;

namespace BlinkReminder
{
    /// <summary>
    /// A simple window that comes to fore and blocks all view
    /// </summary>
    public partial class ViewBlocker : Window, IDisposable
    {
        private Timer timeOfBlock; // Timer to control the window's lifetime
        private CountdownTimer countdownTimer; // Timer to display remaining time

        internal ViewBlocker(long interval, bool isSkippable, string message)
        {
            InitializeComponent();
            SetTimer(interval);
            SetBinding();
            StartViewTimer(interval);
            SetSkippable(isSkippable);
            SetMessage(message);
            PrepareWindow();
        }

        #region Startup helpers
        /// <summary>
        /// Sets the message that appears on screen
        /// </summary>
        /// <param name="msg">This is the msg that will appear</param>
        private void SetMessage(string msg)
        {
            quoteText.Text = msg;
        }

        /// <summary>
        /// Sets if the skip button should appear or not
        /// </summary>
        /// <param name="isSkippable">true or false</param>
        private void SetSkippable(bool isSkippable)
        {
            if (!isSkippable)
            {
                skipButton.IsEnabled = false;
                skipButton.Visibility = Visibility.Hidden;
            }
        }

        private void SetTimer(long duration)
        {
            countdownTimer = new CountdownTimer(duration);
        }

        private void SetBinding()
        {
            blockerGrid.DataContext = countdownTimer;
        }

        /// <summary>
        /// Sets up the window to appear in front of everything
        /// </summary>
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

        /// <summary>
        /// Starts the timer that control the duration of the window's appearance
        /// </summary>
        /// <param name="interval"></param>
        private void StartViewTimer(long interval)
        {
            timeOfBlock = new Timer(interval)
            {
                AutoReset = false
            };
            timeOfBlock.Elapsed += TimeOfBlock_Elapsed;
            timeOfBlock.Start();
        }
        #endregion

        #region Event helpers

        /// <summary>
        /// Properply closes the window and disposes of the timer
        /// </summary>
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

        #endregion

        #region Event methods

        /// <summary>
        /// Called when the timer's countwown is finished
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimeOfBlock_Elapsed(object sender, ElapsedEventArgs e)
        {
            CloseBlockWindow();
        }

        /// <summary>
        /// Called when the user pushes the skip button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            CloseBlockWindow();
        }

        #endregion

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

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ViewBlocker() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
