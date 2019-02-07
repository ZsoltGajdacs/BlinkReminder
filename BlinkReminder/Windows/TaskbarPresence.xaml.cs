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
using System.Windows.Controls;

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

        private const string TOOLTIP_LONG_MSG_BEGIN = "Long break in ";
        private const string TOOLTIP_SHORT_MSG_BEGIN = "Short break in ";
        private const string TOOLTIP_PAUSE_MSG_BEGIN = "Breaks are paused for ";
        private const string TOOLTIP_MSG_END = " minutes";
        private const string TOOLTIP_INDEF_PAUSE = "Breaks are paused until resume is clicked";
        private const string TOOLTIP_LONG_DISABLED = "Long breaks are disabled";
        private const string TOOLTIP_SHORT_DISABLED = "Short breaks are disabled";
        private const int ONE_MINUTE_IN_MS = 60000;

        // Keyboard input catcher
        private KeyboardHook keyTrap;

        // Windows
        private Window settingsWindow;
        private Window blockerWindow;
        private Window aboutWindow;
        private PauseWindow pauseWindow;

        // Stopwatches
        private Stopwatch shortTimerWatch;
        private Stopwatch longTimerWatch;
        private Stopwatch pauseWatch;

        //Timers
        private Timer shortIntervalTimer;
        private Timer longIntervalTimer;
        private Timer tooltipRefreshTimer;
        private Timer pauseTimer;

        //Timer Helpers
        private bool isShortIntervalTimerDone;
        private bool isLongIntervalTimerDone;
        private bool isPaused;
        private TimeSpan pauseTime;

        // Settings singleton
        private UserSettings settings;

        // For identifying the fullscreen window that blocks the blockerWindow
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
            settings = UserSettings.Instance;
            settings.PropertyChanged += UserSettings_PropertyChanged;

            // Stopwatches for pause support
            shortTimerWatch = new Stopwatch();
            longTimerWatch = new Stopwatch();

            // Initialize short, long and taskbar timers
            shortIntervalTimer = new Timer();
            shortIntervalTimer.AutoReset = false;
            shortIntervalTimer.Elapsed += ShortCycleTimer_Elapsed;

            longIntervalTimer = new Timer();
            longIntervalTimer.AutoReset = false;
            longIntervalTimer.Elapsed += LongCycleTimer_Elapsed;

            tooltipRefreshTimer = new Timer();
            tooltipRefreshTimer.AutoReset = true;
            tooltipRefreshTimer.Elapsed += TaskbarTimer_Elapsed;

            isShortIntervalTimerDone = false;
            isLongIntervalTimerDone = false;
        }

        /// <summary>
        /// Starts the default timers at application start
        /// </summary>
        private void StartDefaultTimers()
        {
            // ------------- Short Interval ----------------
            long shortTime = settings.GetShortIntervalMillisecond();

            if (shortTime > 0)
            {
                shortIntervalTimer.Interval = shortTime;
                shortIntervalTimer.Start();
                shortTimerWatch.Start();
            }

            // ------------- Long Interval -----------------
            long longTime = settings.GetLongIntervalMillisecond();

            if (longTime > 0)
            {
                longIntervalTimer.Interval = longTime;
                longIntervalTimer.Start();
                longTimerWatch.Start();
            }

            // ------------- Taskbar time count --------------

            if (longTime > 0)
            {
                tooltipRefreshTimer.Interval = ONE_MINUTE_IN_MS;
                tooltipRefreshTimer.Start();

                SetTaskbarTooltip(TOOLTIP_LONG_MSG_BEGIN + (settings.LongIntervalTime / 60) + TOOLTIP_MSG_END);
            }
            else
            {
                SetTaskbarTooltip(TOOLTIP_LONG_DISABLED);
                DisableTaskbarOption(ref LongBreakStartItem);
            }
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
            if (pauseWindow == null)
            {
                pauseWindow = new PauseWindow();
                pauseWindow.Closed += PauseWindow_Closed;
            }

            pauseTime = TimeSpan.FromMinutes(pauseWindow.ShowDialog());

            isPaused = true;

            // If the user chose timed pause
            if (pauseTime.TotalSeconds > 0)
            {
                if (pauseWatch == null)
                {
                    pauseWatch = new Stopwatch();
                }
                else
                {
                    pauseWatch.Reset();
                }

                SetTaskbarTooltip(TOOLTIP_PAUSE_MSG_BEGIN + TimeToPauseEnd() + TOOLTIP_MSG_END);
                StopTimers();
                StartPauseTimer(pauseTime.TotalMilliseconds);

                pauseWatch.Restart();
            }
            // if this is going on till resume is pushed
            else if (pauseTime.TotalMinutes.Equals(TimeSpan.FromMinutes(-1)))
            {
                PauseItem.Header = "Resume";
                PauseItem.Click -= PauseItem_Click;
                PauseItem.Click += ResumeItem_Click;

                tooltipRefreshTimer.Stop();

                SetTaskbarTooltip(TOOLTIP_INDEF_PAUSE);
                StopTimers();
            }
            // Or the user just canceled
            else
            {
                isPaused = false;
            }
        }

        private void ResumeItem_Click(object sender, RoutedEventArgs e)
        {
            PauseItem.Header = "Pause";
            PauseItem.Click += PauseItem_Click;
            PauseItem.Click -= ResumeItem_Click;

            tooltipRefreshTimer.Start();

            ResumeTimers();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            int[] versionNums = new int[] { MAJVERSION, MINVERSION, REVVERSION };

            if (aboutWindow == null)
            {
                aboutWindow = new AboutWindow(ref versionNums);
                aboutWindow.Closed += AboutWindow_Closed;
                aboutWindow.Show();
            }
            else
            {
                aboutWindow.Activate();
            }
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

            if (settings.ShouldBreakWhenFullScreen && NativeMethods.IsFullscreenAppRunning(out foreProc))
            {
                ResetElaspedTimer();
                RestartStopwatchForElaspedTimer();
            }
            else
            {
                longTimerWatch.Stop();
                ShowViewBlocker(settings.GetLongDisplayMillisecond(), settings.IsLongSkippable, settings.GetLongQuote());
            }
        }

        private void ShortCycleTimer_Elapsed(object sender, EventArgs e)
        {
            isShortIntervalTimerDone = true;

            if (settings.ShouldBreakWhenFullScreen && NativeMethods.IsFullscreenAppRunning(out foreProc))
            {
                ResetElaspedTimer();
                RestartStopwatchForElaspedTimer();
            }
            else
            {
                shortTimerWatch.Stop();
                ShowViewBlocker(settings.GetShortDisplayMillisecond(), settings.IsShortSkippable, settings.GetShortQuote());
            }
        }

        private void TaskbarTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isPaused)
            {
                SetTaskbarTooltip(TOOLTIP_PAUSE_MSG_BEGIN + TimeToPauseEnd() + TOOLTIP_MSG_END);
            }
            else
            {
                SetTaskbarTooltip(TOOLTIP_LONG_MSG_BEGIN + TimeToNextLongBreak() + TOOLTIP_MSG_END);
            }
        }

        private void PauseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            pauseWatch.Stop();
            ResumeTimers();
        }

        #endregion

        #region Window Events

        private void BlockerWindow_Closed(object sender, EventArgs e)
        {
            blockerWindow = null;
            keyTrap?.Dispose(); // Release keyboard trap
            ResetElaspedTimer(); // Restart the clock that started the window that closed
            RestartStopwatchForElaspedTimer(); //Reset the related stopwatch too
        }

        private void SettingsWindow_Closed(object sender, EventArgs e)
        {
            settingsWindow = null;
            SerializeObj(settings, settings.SettingsFilePath, settings.SettingsDirPath);
        }

        private void AboutWindow_Closed(object sender, EventArgs e)
        {
            aboutWindow = null;
        }

        private void PauseWindow_Closed(object sender, EventArgs e)
        {
            pauseWindow = null;
        }

        #endregion

        #region Property changed Events
        private void UserSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // If the app is currently paused don't restart the timers
            if (!isPaused)
            {
                DecideWhichClockToReset(e.PropertyName);
            }
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
                    settingsWindow = new SettingsWindow(ref settings);
                    settingsWindow.Closed += SettingsWindow_Closed;
                    settingsWindow.Show();
                
                }
                else { settingsWindow.Activate(); }
            }));
        }

        #endregion

        #region Timer methods

        /// <summary>
        /// If the user changes time amounts in settings this method is called to set in logic
        /// </summary>
        /// <param name="changedProperty"></param>
        private void DecideWhichClockToReset(string changedProperty)
        {
            switch (changedProperty)
            {
                case "ShortIntervalTime":
                    long shortInterval = settings.GetShortIntervalMillisecond();

                    if (shortInterval > 0)
                    {
                        ResetTimer(ref shortIntervalTimer, shortInterval);
                        shortTimerWatch.Restart();
                    }
                    else
                    {
                        shortIntervalTimer.Stop();
                        shortTimerWatch.Reset();
                    }

                    break;

                case "LongIntervalTime":
                    long longInterval = settings.GetLongIntervalMillisecond();

                    if (longInterval > 0)
                    {
                        ResetTimer(ref longIntervalTimer, longInterval);
                        ResetTimer(ref tooltipRefreshTimer, ONE_MINUTE_IN_MS);
                        longTimerWatch.Restart();

                        EnableTaskbarOption(ref LongBreakStartItem);
                        SetTaskbarTooltip(TOOLTIP_LONG_MSG_BEGIN + (settings.LongIntervalTime / 60) + TOOLTIP_MSG_END);
                    }
                    else
                    {
                        longIntervalTimer.Stop();
                        longTimerWatch.Reset();
                        tooltipRefreshTimer.Stop();
                        SetTaskbarTooltip(TOOLTIP_LONG_DISABLED);
                        DisableTaskbarOption(ref LongBreakStartItem);
                    }
                    
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
        /// Stops the timer and sets the currently set long interval
        /// </summary>
        private void SetBackTooltipTimer()
        {
            SetTaskbarTooltip(TOOLTIP_LONG_MSG_BEGIN + (settings.LongIntervalTime / 60) + TOOLTIP_MSG_END);
            tooltipRefreshTimer.Stop();
        }

        /// <summary>
        /// Stops long and short timers
        /// </summary>
        private void StopTimers()
        {
            shortTimerWatch.Stop();
            longTimerWatch.Stop();

            shortIntervalTimer.Stop();
            longIntervalTimer.Stop();
        }

        /// <summary>
        /// Starts the long and short timers from where they were stopped. 
        /// Continues the stopwatches. 
        /// Handles the case of pause state settings changes
        /// </summary>
        private void ResumeTimers()
        {
            // Come out of paused state
            isPaused = false;

            // Get the latest times from settings
            TimeSpan longIntervalTime = TimeSpan.FromMilliseconds(settings.GetLongIntervalMillisecond());
            TimeSpan shortIntervalTime = TimeSpan.FromMilliseconds(settings.GetShortIntervalMillisecond());

            // Calculate remaining times
            TimeSpan longRemain = longIntervalTime - longTimerWatch.Elapsed;
            TimeSpan shortRemain = shortIntervalTime - shortTimerWatch.Elapsed;

            // Handle the case when the user changed the settings while in pause
            if (longRemain < TimeSpan.Zero)
            {
                longRemain = longIntervalTime;
            }

            if (shortRemain < TimeSpan.Zero)
            {
                shortRemain = shortIntervalTime;
            }

            // Reset them
            ResetTimer(ref shortIntervalTimer, (long)shortRemain.TotalMilliseconds);
            ResetTimer(ref longIntervalTimer, (long)longRemain.TotalMilliseconds);
            SetTaskbarTooltip(TOOLTIP_LONG_MSG_BEGIN + TimeToNextLongBreak() + TOOLTIP_MSG_END);

            shortTimerWatch.Start();
            longTimerWatch.Start();
        }

        /// <summary>
        /// Starts the pauseTimer with the given millisecond amount
        /// </summary>
        /// <param name="amountMillisec"></param>
        private void StartPauseTimer(double amountMillisec)
        {

            pauseTimer = new Timer(amountMillisec)
            {
                AutoReset = false
            };
            pauseTimer.Elapsed += PauseTimer_Elapsed;
            pauseTimer.Start();
        }

        /// <summary>
        /// Resets the timer which generated the latest break event
        /// </summary>
        private void ResetElaspedTimer()
        {
            if (isShortIntervalTimerDone)
            {
                ResetTimer(ref shortIntervalTimer, settings.GetShortIntervalMillisecond());
                shortTimerWatch.Restart();
                isShortIntervalTimerDone = false;
            }

            if (isLongIntervalTimerDone)
            {
                ResetTimer(ref longIntervalTimer, settings.GetLongIntervalMillisecond());
                ResetTimer(ref tooltipRefreshTimer, ONE_MINUTE_IN_MS);
                longTimerWatch.Restart();

                if (settings.GetShortIntervalMillisecond() > 0)
                {
                    ResetTimer(ref shortIntervalTimer, settings.GetShortIntervalMillisecond());
                }

                isLongIntervalTimerDone = false;
            }
        }

        /// <summary>
        /// Resets the stopwatch whose timer has elasped
        /// </summary>
        private void RestartStopwatchForElaspedTimer()
        {
            if (isShortIntervalTimerDone)
            {
                shortTimerWatch.Restart();
            }
            else if (isLongIntervalTimerDone)
            {
                longTimerWatch.Restart();
            }
        }

        /// <summary>
        /// Gives back the remaining minutes till the next long break
        /// </summary>
        /// <returns></returns>
        private int TimeToNextLongBreak()
        {
            TimeSpan timePassed = longTimerWatch.Elapsed;
            TimeSpan totalTime = TimeSpan.FromSeconds(settings.LongIntervalTime);
            TimeSpan remainingTime = totalTime - timePassed;

            return (int)remainingTime.TotalMinutes;
        }

        /// <summary>
        /// Gives back the amount of minutes left until the end of the current pause state
        /// </summary>
        /// <returns></returns>
        private int TimeToPauseEnd()
        {
            TimeSpan timePassed = pauseWatch.Elapsed;
            TimeSpan remainingTime = pauseTime - timePassed;

            return (int)remainingTime.TotalMinutes;
        }

        #endregion

        #region Taskbar manipulation
        /// <summary>
        /// Sets the taskbar tooltip to the given text
        /// </summary>
        /// <param name="text"></param>
        private void SetTaskbarTooltip(string text)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                taskbarIcon.ToolTipText = text;
            }));
        }

        /// <summary>
        /// Disables the given taskbar MenuItem
        /// </summary>
        /// <param name="option"></param>
        private void DisableTaskbarOption(ref MenuItem option)
        {
            option.IsEnabled = false;
        }

        /// <summary>
        /// Enable the given taskbar MenuItem
        /// </summary>
        /// <param name="option"></param>
        private void EnableTaskbarOption(ref MenuItem option)
        {
            option.IsEnabled = true;
        }
        #endregion

        #region Serialization
        /// <summary>
        /// Serializes the given object to the given path
        /// </summary>
        /// <param name="o"></param>
        /// <param name="serializePath"></param>
        private void SerializeObj(Object o, string serializePath, string parentDir)
        {
            DirectoryInfo dirInfo = new DirectoryInfo(parentDir);

            if (!dirInfo.Exists)
            {
                dirInfo.Create();
            }

            try
            {
                IFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(serializePath, FileMode.Create, FileAccess.Write);

                formatter.Serialize(stream, o);
                stream.Close();
            }
            catch (Exception)
            {
                // Should log here
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
                    shortIntervalTimer.Dispose();
                    longIntervalTimer.Dispose();
                    tooltipRefreshTimer.Dispose();
                    pauseTimer.Dispose();
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
