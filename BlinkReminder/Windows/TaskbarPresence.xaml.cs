using BlinkReminder.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;

namespace BlinkReminder.Windows
{
    /// <summary>
    /// Main window of application, actual window is hidden,
    /// only the taskbar icon is active
    /// </summary>
    public partial class TaskbarPresence : Window, IDisposable
    {
        private IContainer components;
        private KeyboardHook kh;

        // Windows
        private Window settingsWindow;
        private Window blockerWindow;

        //Timers
        private Timer shortIntervalTimer;
        private Timer longIntervalTimer;

        //Timer Helpers
        private bool isShortIntervalTimerDone;
        private bool isLongIntervalTimerDone;

        private UserSettings userSettings;


        public TaskbarPresence()
        {
            InitializeComponent();

            userSettings = UserSettings.Instance;
            userSettings.PropertyChanged += UserSettings_PropertyChanged;

            components = new Container();

            isShortIntervalTimerDone = false;
            isLongIntervalTimerDone = false;

            StartDefaultTimers();
        }

        #region Click Events

        private void ShortBreak_Click(object sender, RoutedEventArgs e)
        {
            ShowViewBlocker(userSettings.ShortDisplayTime, userSettings.IsShortSkippable, userSettings.GetShortQuote());
        }

        private void LongBreak_Click(object sender, RoutedEventArgs e)
        {
            ShowViewBlocker(userSettings.LongDisplayTime, userSettings.IsLongSkippable, userSettings.GetLongQuote());
        }

        private void PauseItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Timer Events

        private void LongCycleTimer_Elapsed(object sender, EventArgs e)
        {
            ShowViewBlocker(userSettings.LongDisplayTime, userSettings.IsLongSkippable, userSettings.GetLongQuote());
            isLongIntervalTimerDone = true;
        }

        private void ShortCycleTimer_Elapsed(object sender, EventArgs e)
        {
            ShowViewBlocker(userSettings.ShortDisplayTime, userSettings.IsShortSkippable, userSettings.GetShortQuote());
            isShortIntervalTimerDone = true;
        }

        #endregion

        #region Window Events

        private void BlockerWindow_Closed(object sender, EventArgs e)
        {
            blockerWindow = null;
            kh.Dispose(); // Release keyboard trap
            HandleTimerResetOnWindowClose(); // Restart the clock that started that window that closed
        }

        private void SettingsWindow_Closed(object sender, EventArgs e)
        {
            settingsWindow = null;
        }

        #endregion

        #region Property changed Events
        private void UserSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DecideWhichClockToReset(e.PropertyName);
        }

        #endregion

        #region Window showers

        private void ShowViewBlocker(long interval, bool isSkippable, string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (blockerWindow == null)
                {
                    kh = new KeyboardHook(); // Intercept every key
                    blockerWindow = new ViewBlocker(interval, isSkippable, message);
                    blockerWindow.Closed += BlockerWindow_Closed;
                    blockerWindow.Show();
                
                }
                else { blockerWindow.Activate(); }
            }));
        }

        private void ShowSettings()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (settingsWindow == null)
                {
                    settingsWindow = new SettingsWindow(ref userSettings);
                    settingsWindow.Closed += SettingsWindow_Closed;
                    settingsWindow.Show();
                
                }
                else { settingsWindow.Activate(); }
            }));
        }

        #endregion

        #region Timer methods

        private void StartDefaultTimers()
        {
            shortIntervalTimer = new Timer(userSettings.ShortIntervalTime)
            {
                AutoReset = false
            };
            shortIntervalTimer.Elapsed += ShortCycleTimer_Elapsed;
            shortIntervalTimer.Start();

            longIntervalTimer = new Timer(userSettings.LongIntervalTime)
            {
                AutoReset = false
            };
            longIntervalTimer.Elapsed += LongCycleTimer_Elapsed;
            longIntervalTimer.Start();

            components.Add(shortIntervalTimer);
            components.Add(longIntervalTimer);
        }

        private void DecideWhichClockToReset(string changedProperty)
        {
            switch (changedProperty)
            {
                case "ShortIntervalTime":
                    ResetTimer(ref shortIntervalTimer, userSettings.ShortIntervalTime);
                    break;

                case "LongIntervalTime":
                    ResetTimer(ref longIntervalTimer, userSettings.LongIntervalTime);
                    break;

                default:
                    break;
            }
        }

        private void ResetTimer(ref Timer timer, long time)
        {
            timer.Stop();
            timer.Interval = time;
            timer.Start();
        }

        #endregion

        #region Timer Helpers

        private void HandleTimerResetOnWindowClose()
        {
            if (isShortIntervalTimerDone)
            {
                ResetTimer(ref shortIntervalTimer, userSettings.ShortIntervalTime);
            }
            else if (isLongIntervalTimerDone)
            {
                ResetTimer(ref longIntervalTimer, userSettings.LongIntervalTime);
            }
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
                    components.Dispose();
                    taskbarIcon.Dispose();

                    if (kh != null)
                    {
                        kh.Dispose();
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TaskbarPresence() {
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
