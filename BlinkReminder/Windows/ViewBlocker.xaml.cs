using System;
using System.Windows;
using System.Timers;
using BlinkReminder.Windows.Support;
using BlinkReminder.Helpers;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Controls;

namespace BlinkReminder
{
    /// <summary>
    /// A simple window that comes to fore and blocks all view
    /// </summary>
    public partial class ViewBlocker : Window, IDisposable
    {
        private static int BASE_CONTROL_HEIGHT = 31;
        private static int BASE_DISTANCE_FROM_EDGE = 15;
        private static int BASE_WINDOW_HEIGHT = 150;
        private static int BASE_WINDOW_WIDTH = 400;
        private static int BASE_WINDOW_WIDTH_PADDING = 60;
        private static int BASE_BTN_WIDTH = 120;
        private static int BASE_FONT_SIZE = 16;
        private readonly int QUOTE_TEXT_WIDTH;

        private Timer timeOfBlock; // Timer to control the window's lifetime
        private CountdownTimer countdownTimer; // Timer to display remaining time

        private Timer timeToLock; // Timer to lock the machine sometime after the block starts

        // Window size properties
        public double ControlHeight { get; set; }
        public double BtnWidth { get; set; }
        public double Distance { get; set; }

        internal ViewBlocker(TimeSpan interval, double scaling, bool isSkippable, 
                                bool isFullscreen, bool isLongBreakLocksScreen, 
                                bool isLongBreak, string message)
        {
            InitializeComponent();

            QUOTE_TEXT_WIDTH = MeasureString(message, ref quoteText, scaling).Width;

            SetWindowSize(isFullscreen, scaling);
            SetTimer(interval);
            SetBinding();
            StartViewTimer(interval);
            StartLockTimer(isLongBreakLocksScreen, isLongBreak, 5000);
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

        /// <summary>
        /// Sets the internal timer of the window
        /// </summary>
        private void SetTimer(TimeSpan duration)
        {
            countdownTimer = new CountdownTimer(duration);
        }

        /// <summary>
        /// Sets binding of window parts
        /// </summary>
        private void SetBinding()
        {
            blockerGrid.DataContext = this;
            blockerStack.DataContext = countdownTimer;
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
        private void StartViewTimer(TimeSpan interval)
        {
            timeOfBlock = new Timer(interval.TotalMilliseconds)
            {
                AutoReset = false
            };
            timeOfBlock.Elapsed += TimeOfBlock_Elapsed;
            timeOfBlock.Start();
        }
        
        /// <summary>
        /// Sets window size in case the user chose small screen breaks
        /// </summary>
        private void SetWindowSize(bool isFullscreen, double scaling)
        {
            ControlHeight = BASE_CONTROL_HEIGHT;
            BtnWidth = BASE_BTN_WIDTH;
            Distance = BASE_DISTANCE_FROM_EDGE;

            if (!isFullscreen)
            {
                this.WindowState = WindowState.Normal;
                this.WindowStartupLocation = WindowStartupLocation.Manual;

                // Set width and height of window via scaling set by user
                double w = (QUOTE_TEXT_WIDTH * scaling) + (BASE_WINDOW_WIDTH_PADDING * scaling);
                w = w >= BASE_WINDOW_WIDTH ? w : BASE_WINDOW_WIDTH;
                this.Width = w;

                double h = BASE_WINDOW_HEIGHT * scaling;
                this.Height = h;

                // Set font and btn so it scales as well
                /*if (scaling < 1)
                {
                    this.FontSize = BASE_FONT_SIZE * (scaling + (1 - scaling) / 2);
                }
                else if (scaling > 1)
                {
                    this.FontSize = BASE_FONT_SIZE * (scaling - (scaling - 1) / 2);
                }*/

                this.FontSize = BASE_FONT_SIZE * scaling;
                BtnWidth *= scaling;
                ControlHeight *= scaling;

                // Set window position to the lower right edge of screen
                this.Left = ScreenSizeInfo.CalculateLeftEdgeOfWindow(w);
                this.Top = ScreenSizeInfo.CalculateTopEdgeOfWindow(h);
            }
            else
            {
                Distance = Distance * 2;
            }
        }
        
        /// <summary>
        /// Starts the timer responsible for locking the machine when the given time is done
        /// </summary>
        private void StartLockTimer(bool isLongBreakLocksScreen, bool isLongBreak, double millisecondsToLock)
        {
            if (isLongBreakLocksScreen && isLongBreak)
            {
                timeToLock = new Timer(millisecondsToLock);
                timeToLock.AutoReset = false;
                timeToLock.Elapsed += TimeToLock_Elapsed;
                timeToLock.Start();
            }
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
        private void TimeOfBlock_Elapsed(object sender, ElapsedEventArgs e)
        {
            CloseBlockWindow();
        }

        /// <summary>
        /// Called when the user pushes the skip button
        /// </summary>
        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            CloseBlockWindow();
        }

        private void LockButton_Click(object sender, RoutedEventArgs e)
        {
            Locker.LockWorkStation();
        }

        /// <summary>
        /// Called when the timer to control when to lock the machine finishes
        /// </summary>
        private void TimeToLock_Elapsed(object sender, ElapsedEventArgs e)
        {
            Locker.LockWorkStation();
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Measures the given string length for the given textblock, whith the given scaling
        /// </summary>
        private System.Drawing.Size MeasureString(string candidate, ref TextBlock textblock, double scaling)
        {
            var formattedText = new FormattedText(
                candidate,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(textblock.FontFamily, textblock.FontStyle, textblock.FontWeight, textblock.FontStretch),
                BASE_FONT_SIZE,
                System.Windows.Media.Brushes.White,
                new NumberSubstitution(),
                VisualTreeHelper.GetDpi(this).DpiScaleX);

            return new System.Drawing.Size((int)formattedText.Width, (int)formattedText.Height);
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
                    timeToLock.Dispose();
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
