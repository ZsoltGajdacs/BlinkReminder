using BlinkReminder.Settings;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using BlinkReminder.Helpers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace BlinkReminder.Windows
{
    /// <summary>
    /// Main window of application, actual window is hidden,
    /// only the taskbar icon is active
    /// </summary>
    public partial class TaskbarPresence : Window, IDisposable
    {
        // Consts
        private readonly int MAJVERSION = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileMajorPart;
        private readonly int MINVERSION = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileMinorPart;
        private readonly int REVVERSION = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileBuildPart;
        private const string TOOLTIP_MSG_BEGIN = "Time until next long break: ";
        private const string TOOLTIP_MSG_END = " minutes";
        private const int ONE_MINUTE_IN_MS = 60000;

        // Component holder for disposal
        private IContainer components;

        // Keyboard input catcher
        private KeyboardHook keyTrap;

        // Windows
        private Window settingsWindow;
        private Window blockerWindow;

        //Timers
        private Timer shortIntervalTimer;
        private Timer longIntervalTimer;
        private Timer tooltipRefreshTimer;

        //Timer Helpers
        private bool isShortIntervalTimerDone;
        private bool isLongIntervalTimerDone;
        private long timeToNextLongBreak; // Expressed in minutes

        // Settings singleton
        private UserSettings userSettings;

        // For identifying the fullscreen window that can block the blockerWindow
        Process foreProc;

        public TaskbarPresence()
        {
            InitializeComponent();
            SetDefaultValues();
            StartDefaultTimers();
        }

        #region Startup support
        private void SetDefaultValues()
        {
            // Get Single settings instance and subscribe to it's event
            userSettings = UserSettings.Instance;
            userSettings.PropertyChanged += UserSettings_PropertyChanged;

            components = new Container();

            isShortIntervalTimerDone = false;
            isLongIntervalTimerDone = false;

            RefreshLongIntervalTimeCount();
            SetTaskbarTooltip(TOOLTIP_MSG_BEGIN + timeToNextLongBreak + TOOLTIP_MSG_END);
        }
        #endregion

        #region Click Events

        private void ShortBreak_Click(object sender, RoutedEventArgs e)
        {
            ShortCycleTimer_Elapsed(sender, e);
        }

        private void LongBreak_Click(object sender, RoutedEventArgs e)
        {
            LongCycleTimer_Elapsed(sender, e);
        }

        private void PauseItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            string versionText = "Version: " + MAJVERSION + "." + MINVERSION + "." + REVVERSION;
            MessageBox.Show(versionText, "Current product version", MessageBoxButton.OK, MessageBoxImage.None, MessageBoxResult.OK, MessageBoxOptions.DefaultDesktopOnly);
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Timer Events

        private void LongCycleTimer_Elapsed(object sender, EventArgs e)
        {
            isLongIntervalTimerDone = true;
            SetBackTooltipTimer();

            if (userSettings.ShouldBreakWhenFullScreen && NativeMethods.IsFullscreenAppRunning(out foreProc))
            {
                ResetElaspedTimer();
            }
            else
            {
                ShowViewBlocker(userSettings.GetLongDisplayMillisecond(), userSettings.IsLongSkippable, userSettings.GetLongQuote());
            }
        }

        private void ShortCycleTimer_Elapsed(object sender, EventArgs e)
        {
            isShortIntervalTimerDone = true;

            if (userSettings.ShouldBreakWhenFullScreen && NativeMethods.IsFullscreenAppRunning(out foreProc))
            {
                ResetElaspedTimer();
            }
            else
            {
                ShowViewBlocker(userSettings.GetShortDisplayMillisecond(), userSettings.IsShortSkippable, userSettings.GetShortQuote());
            }
        }

        private void TaskbarTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (timeToNextLongBreak > 0)
            {
                --timeToNextLongBreak; // One minute passed
            }

            SetTaskbarTooltip(TOOLTIP_MSG_BEGIN + timeToNextLongBreak + TOOLTIP_MSG_END);
        }

        #endregion

        #region Window Events

        private void BlockerWindow_Closed(object sender, EventArgs e)
        {
            blockerWindow = null;
            keyTrap?.Dispose(); // Release keyboard trap
            ResetElaspedTimer(); // Restart the clock that started the window that closed
        }

        private void SettingsWindow_Closed(object sender, EventArgs e)
        {
            settingsWindow = null;
            SerializeObj(userSettings, UserSettings.SETTINGSFILEPATH);
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
                    keyTrap = new KeyboardHook(); // Intercept every key
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

        /// <summary>
        /// Starts the default timers at application start
        /// </summary>
        private void StartDefaultTimers()
        {
            // Short Interval
            shortIntervalTimer = new Timer(userSettings.GetShortIntervalMillisecond())
            {
                AutoReset = false
            };
            shortIntervalTimer.Elapsed += ShortCycleTimer_Elapsed;
            shortIntervalTimer.Start();

            // Long Interval
            longIntervalTimer = new Timer(userSettings.GetLongIntervalMillisecond())
            {
                AutoReset = false
            };
            longIntervalTimer.Elapsed += LongCycleTimer_Elapsed;
            longIntervalTimer.Start();

            // Taskbar count
            tooltipRefreshTimer = new Timer(ONE_MINUTE_IN_MS) // Fires every minute
            {
                AutoReset = true
            };
            tooltipRefreshTimer.Elapsed += TaskbarTimer_Elapsed;
            tooltipRefreshTimer.Start();

            // Add to Component holder for proper disposal later
            components.Add(shortIntervalTimer);
            components.Add(longIntervalTimer);
            components.Add(tooltipRefreshTimer);
        }

        /// <summary>
        /// Based on the changed properties name it resets the timer related to it
        /// </summary>
        /// <param name="changedProperty"></param>
        private void DecideWhichClockToReset(string changedProperty)
        {
            switch (changedProperty)
            {
                case "ShortIntervalTime":
                    ResetTimer(ref shortIntervalTimer, userSettings.GetShortIntervalMillisecond());
                    break;

                case "LongIntervalTime":
                    ResetTimer(ref longIntervalTimer, userSettings.GetLongIntervalMillisecond());
                    RefreshLongIntervalTimeCount();
                    SetTaskbarTooltip(TOOLTIP_MSG_BEGIN + timeToNextLongBreak + TOOLTIP_MSG_END);
                    ResetTimer(ref tooltipRefreshTimer, ONE_MINUTE_IN_MS);
                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Resets the given timer to the given time
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="time"></param>
        private void ResetTimer(ref Timer timer, long time)
        {
            timer.Stop();
            timer.Interval = time;
            timer.Start();
        }

        /// <summary>
        /// Stops the timer and sets the current long interval
        /// </summary>
        private void SetBackTooltipTimer()
        {
            RefreshLongIntervalTimeCount();
            SetTaskbarTooltip(TOOLTIP_MSG_BEGIN + timeToNextLongBreak + TOOLTIP_MSG_END);
            tooltipRefreshTimer.Stop();
        }

        #endregion

        #region Timer Helpers

        /// <summary>
        /// Resets the timer which generated the latest break event
        /// </summary>
        private void ResetElaspedTimer()
        {
            if (isShortIntervalTimerDone)
            {
                ResetTimer(ref shortIntervalTimer, userSettings.GetShortIntervalMillisecond());
                isShortIntervalTimerDone = false;
            }

            if (isLongIntervalTimerDone)
            {
                ResetTimer(ref longIntervalTimer, userSettings.GetLongIntervalMillisecond());
                ResetTimer(ref tooltipRefreshTimer, ONE_MINUTE_IN_MS);
                isLongIntervalTimerDone = false;
            }
        }

        /// <summary>
        /// Gets the latest long interval setting from userSettings
        /// </summary>
        private void RefreshLongIntervalTimeCount()
        {
            timeToNextLongBreak = userSettings.LongIntervalTime / 60;
        }

        #endregion

        #region Taskbar manipulation
        private void SetTaskbarTooltip(string text)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                taskbarIcon.ToolTipText = text;
            }));
        }
        #endregion

        #region Serialization
        /// <summary>
        /// Serializes the given object to the given path
        /// </summary>
        /// <param name="o"></param>
        /// <param name="serializePath"></param>
        private void SerializeObj(Object o, string serializePath)
        {
            IFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(serializePath, FileMode.Create, FileAccess.Write);

            formatter.Serialize(stream, o);
            stream.Close();
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

                    if (keyTrap != null)
                    {
                        keyTrap.Dispose();
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
