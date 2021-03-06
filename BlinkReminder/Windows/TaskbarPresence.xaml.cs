﻿using BlinkReminder.Settings;
using System;
using System.Timers;
using System.Windows;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Controls;
using System.Collections.Generic;
using Microsoft.Win32;
using BlinkReminder.Windows.Controls;
using Hardcodet.Wpf.TaskbarNotification;
using BlinkReminder.Update;
using BlinkReminder.Helper.Hooks;
using BlinkReminder.Timers;
using BlinkReminder.Helpers.FileHandlers;
using BlinkReminder.Helpers.ScreenInfo;

namespace BlinkReminder.Windows
{
    /// <summary>
    /// Main window of application, actual window is hidden,
    /// only the taskbar icon is active
    /// </summary>
    public partial class TaskbarPresence : Window, IDisposable
    {
        #region Data members
        // Consts
        private readonly int MAJVERSION = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileMajorPart;
        private readonly int MINVERSION = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileMinorPart;
        private readonly int REVVERSION = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileBuildPart;

        private const string TOOLTIP_LONG_MSG = " minutes to long break";
        private const string TOOLTIP_SHORT_MSG = " minutes to short break";
        private const string TOOLTIP_PAUSE_MSG = " minutes until pause end";
        private const string TOOLTIP_INDEF_PAUSE = "Breaks are paused until resume is clicked";
        private const string TOOLTIP_LONG_DISABLED = "Long breaks are disabled";
        private const string TOOLTIP_SHORT_DISABLED = "Short breaks are disabled";
        private const int HALF_MINUTE_IN_MS = 30000;

        private const string PRE_LONG_BREAK_POPUP_TEXT = "Time for a long break!";
        private const string PRE_SHORT_BREAK_POPUP_TEXT = "Time for a short break!";

        private const string UPDATE_AVAILABLE_TEXT = "There is an update available, would you like to download it?";
        private const string UPDATE_AVAILABLE_CAPTION = "Update available";

        // Keyboard input catcher
        private KeyboardHook keyTrap;

        // Windows
        private Window settingsWindow;
        private Window blockerWindow;
        private Window aboutWindow;
        private PauseWindow pauseWindow;

        // Counters for pause
        private TimeSpan shortBreakLengthSoFar;
        private TimeSpan longBreakLengthSoFar;
        private TimeSpan pauseLengthSoFar;

        // Timers
        private Timer shortIntervalTimer;
        private Timer longIntervalTimer;
        private Timer minuteTimer;
        private Timer pauseTimer;

        // Timer Helpers
        private bool isShortIntervalTimerDone;
        private bool isLongIntervalTimerDone;
        private bool isPaused;
        private TimeSpan pauseTotalLength;

        // Counters to know how many times the user postponed a break
        private int shortBreakPostponeCount;
        private int longBreakPostponeCount;

        // Stopwatch to know how long is the machine locked
        private Stopwatch lockWatch;

        // Pre-break popup
        private BreakNotificationPopup breakPopup;

        // Update checker
        private UpdateCheck updater;

        // Settings singleton
        private UserSettings settings;

        // For identifying the fullscreen window that blocks the blockerWindow
        Process foreProc;

        // NLog logging
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        #endregion

        #region CTOR
        public TaskbarPresence()
        {
            InitializeComponent();
            SetDefaultValues();
            StartDefaultTimers();
            CheckForUpdate();

            logger.Debug("Startup successful");
        }
        #endregion

        #region Startup support
        private void SetDefaultValues()
        {
            // Get single settings instance and subscribe to it's event
            settings = UserSettings.Instance;
            settings.PropertyChanged += UserSettings_PropertyChanged;

            // Initialize short, long, pause and minute timers
            shortIntervalTimer = new Timer();
            shortIntervalTimer.AutoReset = false;
            shortIntervalTimer.Elapsed += ShortCycleTimer_Elapsed;

            longIntervalTimer = new Timer();
            longIntervalTimer.AutoReset = false;
            longIntervalTimer.Elapsed += LongCycleTimer_Elapsed;

            pauseTimer = new Timer();
            pauseTimer.AutoReset = false;
            pauseTimer.Elapsed += PauseTimer_Elapsed;

            minuteTimer = new Timer();
            minuteTimer.AutoReset = true;
            minuteTimer.Elapsed += MinuteTimer_Elasped;

            // Set up update checker
            updater = new UpdateCheck(new int[] { MAJVERSION, MINVERSION, REVVERSION });

            // Subscribe to workstation lock event
            Microsoft.Win32.SystemEvents.SessionSwitch += new Microsoft.Win32.SessionSwitchEventHandler(SystemEvents_SessionSwitch);

            // Subscribe to sleep event - Unsubscribe in Dispose!
            // Microsoft.Win32.SystemEvents.PowerModeChanged += OnPowerChange;

            // Create stopwatch
            lockWatch = new Stopwatch();

            // Set up the pre-break popup
            breakPopup = new BreakNotificationPopup();
            TaskbarIcon.AddBalloonClosingHandler(breakPopup, OnBalloonClosing);
        }

        /// <summary>
        /// Starts the default timers at application start
        /// </summary>
        private void StartDefaultTimers()
        {
            // ------------- Short Interval ----------------
            TimeSpan shortIntervalTimeSpan = settings.ShortIntervalTime;
            SubstractForNotification(ref shortIntervalTimeSpan);
            double shortTime = shortIntervalTimeSpan.TotalMilliseconds;

            if (shortTime > 0)
            {
                shortIntervalTimer.Interval = shortTime;
                shortIntervalTimer.Start();
            }

            // ------------- Long Interval -----------------
            TimeSpan longIntervalTimeSpan = settings.LongIntervalTime;
            SubstractForNotification(ref longIntervalTimeSpan);
            double longTime = longIntervalTimeSpan.TotalMilliseconds;

            if (longTime > 0)
            {
                longIntervalTimer.Interval = longTime;
                longIntervalTimer.Start();
            }

            // ------------- Minute counter for pause and taskbar --------------

            minuteTimer.Interval = HALF_MINUTE_IN_MS;
            minuteTimer.Start();

            if (longTime > 0)
            {
                SetTaskbarTooltip(Math.Round(settings.LongIntervalTime.TotalMinutes, 1) + TOOLTIP_LONG_MSG);
            }
            else
            {
                SetTaskbarTooltip(TOOLTIP_LONG_DISABLED);
                DisableTaskbarOption(LongBreakStartItem);
            }
        }
        
        /// <summary>
        /// Checks for program update and notifies the user if found
        /// </summary>
        private async void CheckForUpdate()
        {
            string result = await updater.GetUpdateUrl();
            
            if (result.StartsWith("https"))
            {
                MessageBoxResult mBoxResult = MessageBox.Show(
                    UPDATE_AVAILABLE_TEXT, UPDATE_AVAILABLE_CAPTION, 
                    MessageBoxButton.YesNo, MessageBoxImage.Question, 
                    MessageBoxResult.Yes, MessageBoxOptions.DefaultDesktopOnly);

                if (mBoxResult == MessageBoxResult.Yes)
                {
                    UpdateHandler updater = new UpdateHandler();
                    bool isOkToLaunch = await updater.DownloadUpdate(result);

                    if (isOkToLaunch)
                    {
                        updater.RunUpdate();
                    }
                }
            }
        }
        #endregion

        #region Click Events

        /// <summary>
        /// Used for dev purposes only
        /// </summary>
        private void Test_Click(object sender, RoutedEventArgs e)
        {
            BreakNotificationPopup breakPopup = new BreakNotificationPopup();
            taskbarIcon.ShowCustomBalloon(breakPopup, 
                System.Windows.Controls.Primitives.PopupAnimation.Fade, 10000);
        }

        private void LongBreak_Click(object sender, RoutedEventArgs e)
        {
            bool isLongBreak = true;
            ShowViewBlocker(settings.LongDisplayTime, settings.Scaling,
                    settings.IsLongSkippable, settings.IsFullscreenBreak,
                    settings.IsLongBreakLocksScreen, isLongBreak, settings.GetLongQuote());
        }

        private void PauseItem_Click(object sender, RoutedEventArgs e)
        {
            if (pauseWindow == null)
            {
                pauseWindow = new PauseWindow();
                pauseWindow.Closed += PauseWindow_Closed;
            }
            else
            {
                pauseWindow.Activate();
            }

            pauseTotalLength = TimeSpan.FromMinutes(pauseWindow.ShowDialog());

            isPaused = true;

            // If the user chose timed pause
            if (pauseTotalLength.TotalSeconds > 0)
            {
                // Zero out the counter to know how long the pause should be going on
                TimerHandler.ResetCounterTime(ref pauseLengthSoFar);

                ActivateResumeBtn();
                SetTaskbarTooltip(TimeToPauseEnd() + TOOLTIP_PAUSE_MSG);
                PauseTimers(true);
            }
            // if this is going on till resume is pushed
            else if (pauseTotalLength.TotalMinutes.Equals(TimeSpan.FromMinutes(-1)))
            {
                ActivateResumeBtn();
                SetTaskbarTooltip(TOOLTIP_INDEF_PAUSE);
                PauseTimers(false);
            }
            // Or the user just canceled
            else
            {
                isPaused = false;
            }
        }

        private void ResumeItem_Click(object sender, RoutedEventArgs e)
        {
            ActivatePauseBtn();
            ResumeTimers();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            ShowSettings();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            ShowAbout();
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
            SetTaskbarTooltip((settings.LongIntervalTime.TotalMinutes) + TOOLTIP_LONG_MSG);

            if (settings.ShouldBreakWhenFullScreen && FullScreenDetect.IsFullscreenAppRunning(out foreProc))
            {
                ResetTimers(settings.LongIntervalTime, settings.ShortIntervalTime);
            }
            else
            {
                bool isLongBreak = true; // Needed for the decision to lock the machine

                // Handle the case when the user wants to have pre-break notifications
                if (settings.IsNotificationEnabled)
                {
                    ShowNotificationPopup(PRE_LONG_BREAK_POPUP_TEXT, longBreakPostponeCount);
                }
                else
                {
                    ShowViewBlocker(settings.LongDisplayTime, settings.Scaling,
                    settings.IsLongSkippable, settings.IsFullscreenBreak,
                    settings.IsLongBreakLocksScreen, isLongBreak, settings.GetLongQuote());
                }
            }
        }

        private void ShortCycleTimer_Elapsed(object sender, EventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                isShortIntervalTimerDone = true;
                bool isPopupOpen = false;

                if (taskbarIcon.CustomBalloon != null)
                {
                    isPopupOpen = taskbarIcon.CustomBalloon.IsOpen;
                }

                if (settings.ShouldBreakWhenFullScreen && FullScreenDetect.IsFullscreenAppRunning(out foreProc)
                    || isPopupOpen)
                {
                    ResetTimers(settings.LongIntervalTime, settings.ShortIntervalTime);
                }
                else
                {
                    if (settings.IsNotificationEnabled)
                    {
                        ShowNotificationPopup(PRE_SHORT_BREAK_POPUP_TEXT, shortBreakPostponeCount);
                    }
                    else
                    {
                        bool isLongBreak = false; // Needed for the decision to lock the machine
                        ShowViewBlocker(settings.ShortDisplayTime, settings.Scaling,
                            settings.IsShortSkippable, settings.IsFullscreenBreak,
                            settings.IsLongBreakLocksScreen, isLongBreak, settings.GetShortQuote());
                    }
                    
                }
            }));
        }

        private void MinuteTimer_Elasped(object sender, ElapsedEventArgs e)
        {
            if (isPaused)
            {
                SetTaskbarTooltip(TimeToPauseEnd() + TOOLTIP_PAUSE_MSG);
                TimerHandler.IncrementCounterTime(ref pauseLengthSoFar, HALF_MINUTE_IN_MS);
            }
            else
            {
                SetTaskbarTooltip(TimeToNextLongBreak() + TOOLTIP_LONG_MSG);
                TimerHandler.IncrementCounterTime(ref shortBreakLengthSoFar, HALF_MINUTE_IN_MS);
                TimerHandler.IncrementCounterTime(ref longBreakLengthSoFar, HALF_MINUTE_IN_MS);
            }
        }

        private void PauseTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            TimerHandler.ResetCounterTime(ref pauseLengthSoFar);
            ResumeTimers();
            ActivatePauseBtn();
        }

        #endregion

        #region Window Events

        private void BlockerWindow_Closed(object sender, EventArgs e)
        {
            blockerWindow = null;
            keyTrap?.Dispose(); // Release keyboard trap
            ResetTimers(settings.LongIntervalTime, settings.ShortIntervalTime);

            // Set back the taskbarWindow to be the main one
            Application.Current.MainWindow = this;
        }

        private void SettingsWindow_Closed(object sender, EventArgs e)
        {
            settingsWindow = null;
            Serializer.JsonObjectSerialize<UserSettings>(settings.SettingsDirPath, settings.SettingsFilePath, ref settings, DoBackup.Yes);
        }

        private void AboutWindow_Closed(object sender, EventArgs e)
        {
            aboutWindow = null;
        }

        private void PauseWindow_Closed(object sender, EventArgs e)
        {
            pauseWindow = null;
        }

        private void OnBalloonClosing(object sender, RoutedEventArgs e)
        {
            bool isLongBreak = true;

            if (breakPopup.ShouldPostponeBreak)
            {
                if (isLongIntervalTimerDone)
                {
                    ++longBreakPostponeCount;
                }
                else if (isShortIntervalTimerDone)
                {
                    ++shortBreakPostponeCount;
                }

                // The timer that called for restart is reset with the postpone length
                ResetTimers(settings.PostponeLength, settings.PostponeLength);
            }
            else
            {
                ResetPostponeCount();

                if (breakPopup.ShouldStartBreak)
                {
                    if (isLongIntervalTimerDone)
                    {
                        ShowViewBlocker(settings.LongDisplayTime, settings.Scaling,
                            settings.IsLongSkippable, settings.IsFullscreenBreak,
                            settings.IsLongBreakLocksScreen, isLongBreak, settings.GetLongQuote());
                    }
                    else if (isShortIntervalTimerDone)
                    {
                        isLongBreak = false;
                        ShowViewBlocker(settings.ShortDisplayTime, settings.Scaling,
                            settings.IsShortSkippable, settings.IsFullscreenBreak,
                            settings.IsLongBreakLocksScreen, isLongBreak, settings.GetShortQuote());
                    }
                }
                else
                {
                    ResetTimers(settings.LongIntervalTime, settings.ShortIntervalTime);
                }
            }
        }

        #endregion

        #region System Events
        /// <summary>
        /// Happens when the user lockes the workstation, determines the timers to restart based on 
        /// lock time
        /// </summary>
        private void SystemEvents_SessionSwitch(object sender, SessionSwitchEventArgs e)
        {
            String longInterval = "LongIntervalTime";
            String shortInterval = "ShortIntervalTime";

            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                lockWatch.Restart();

                if (!isPaused)
                {
                    PauseTimers(false);
                }

                logger.Debug("Machine locked");
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                lockWatch.Stop();
                String clockName = String.Empty;

                if (lockWatch.Elapsed > settings.LockLengthTimeExtent)
                {
                    clockName = longInterval;
                }
                else
                {
                    clockName = shortInterval;
                }

                if (!isPaused)
                {
                    // Order is important here! First the timers are resumed from where they
                    // were before. Only after then can the reset be done based on lock length
                    ResumeTimers();
                    DecideWhichClockToReset(clockName);

                    // If this is considered a long break then 
                    // the short break timer should too be reset
                    if (clockName.Equals(longInterval))
                    {
                        DecideWhichClockToReset(shortInterval);
                    }
                }

                logger.Debug("Machine unlocked, " + clockName + " reset");
            }
        }

        private void OnPowerChange(object s, PowerModeChangedEventArgs e)
        {
            // Not needed at this time.
            // Here for possible future usage
            /*
            switch (e.Mode)
            {
                case PowerModes.Resume:
                    break;

                case PowerModes.Suspend:
                    break;
            }
            */
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

        private void ShowViewBlocker(TimeSpan interval, double scaling, bool isSkippable, bool isFullscreen, bool isLongBreakLocksScreen, bool isLongBreak, string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (blockerWindow == null)
                {
                    if (settings.IsFullscreenBreak)
                    {
                        keyTrap = new KeyboardHook(); // Intercept every key
                    }
                    blockerWindow = new ViewBlocker(interval, scaling, isSkippable, isFullscreen, isLongBreakLocksScreen, isLongBreak, message);
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

        private void ShowAbout()
        {
            if (aboutWindow == null)
            {
                aboutWindow = new AboutWindow(ref updater);
                aboutWindow.Closed += AboutWindow_Closed;
                aboutWindow.Show();
            }
            else
            {
                aboutWindow.Activate();
            }
        }

        #endregion

        #region Popup handling

        /// <summary>
        /// Shows the pre-notification popup with the given text. PostponeCount is for the internal 
        /// postpone limiter
        /// </summary>
        private void ShowNotificationPopup(string popupText, int popupPostponeCount)
        {
            breakPopup.SetValues(popupText, popupPostponeCount);
            taskbarIcon.ShowCustomBalloon(breakPopup,
                System.Windows.Controls.Primitives.PopupAnimation.Slide,
                (int)settings.NotificationLength.TotalMilliseconds);
        }

        #endregion

        #region Timer methods

        /// <summary>
        /// If the user changes time amounts in settings this method is called to set in logic
        /// </summary>
        private void DecideWhichClockToReset(string changedProperty)
        {
            switch (changedProperty)
            {
                case "ShortIntervalTime":
                    TimeSpan shortIntervalTimeSpan = settings.ShortIntervalTime;
                    SubstractForNotification(ref shortIntervalTimeSpan);
                    double shortInterval = shortIntervalTimeSpan.TotalMilliseconds;

                    if (shortInterval > 0)
                    {
                        TimerHandler.RestartTimer(ref shortIntervalTimer, shortInterval);
                    }
                    // If disabled
                    else
                    {
                        shortIntervalTimer.Stop();
                    }

                    TimerHandler.ResetCounterTime(ref shortBreakLengthSoFar);

                    break;

                case "LongIntervalTime":
                    TimeSpan longIntervalTimeSpan = settings.LongIntervalTime;
                    SubstractForNotification(ref longIntervalTimeSpan);

                    double longInterval = longIntervalTimeSpan.TotalMilliseconds;

                    if (longInterval > 0)
                    {
                        TimerHandler.RestartTimer(ref longIntervalTimer, longInterval);

                        EnableTaskbarOption(LongBreakStartItem);
                        SetTaskbarTooltip(Math.Round(settings.LongIntervalTime.TotalMinutes, 1) + TOOLTIP_LONG_MSG);
                    }
                    // If disabled
                    else
                    {
                        longIntervalTimer.Stop();
                        SetTaskbarTooltip(TOOLTIP_LONG_DISABLED);
                        DisableTaskbarOption(LongBreakStartItem);
                    }

                    TimerHandler.ResetCounterTime(ref longBreakLengthSoFar);

                    break;

                default:
                    break;
            }

            // Since there were changes restart the minute timer as well
            TimerHandler.RestartTimer(ref minuteTimer, HALF_MINUTE_IN_MS);
        }

        /// <summary>
        /// Pauses the short and long timers if timed pause.
        /// Short, long and minute timers if not timed pause.
        /// </summary>
        private void PauseTimers(bool isTimed)
        {
            if (isTimed)
            {
                List<Timer> timersToStop = new List<Timer>() { shortIntervalTimer, longIntervalTimer };
                TimerHandler.StopTimers(ref timersToStop);

                TimerHandler.RestartTimer(ref pauseTimer, pauseTotalLength.TotalMilliseconds);
            }
            else
            {
                List<Timer> timersToStop = new List<Timer>() { shortIntervalTimer, longIntervalTimer, minuteTimer };
                TimerHandler.StopTimers(ref timersToStop);
            }
        }

        /// <summary>
        /// Starts the long and short timers from where they were stopped. 
        /// Handles the case of pause state settings changes
        /// </summary>
        private void ResumeTimers()
        {
            // Come out of paused state
            isPaused = false;

            // Get the latest times from settings
            TimeSpan longIntervalTime = TimeSpan.FromMilliseconds(settings.LongIntervalTime.TotalMilliseconds);
            TimeSpan shortIntervalTime = TimeSpan.FromMilliseconds(settings.ShortIntervalTime.TotalMilliseconds);

            // Calculate remaining times
            TimeSpan longRemain = longIntervalTime - longBreakLengthSoFar;
            TimeSpan shortRemain = shortIntervalTime - shortBreakLengthSoFar;

            // Handle the case when the user changed the settings while in pause
            if (longRemain < TimeSpan.Zero)
            {
                longRemain = longIntervalTime;
            }

            if (shortRemain < TimeSpan.Zero)
            {
                shortRemain = shortIntervalTime;
            }

            // Account for possible pre-break notification
            SubstractForNotification(ref longRemain);
            SubstractForNotification(ref shortRemain);

            // Reset them
            TimerHandler.RestartTimer(ref shortIntervalTimer, shortRemain.TotalMilliseconds);
            TimerHandler.RestartTimer(ref longIntervalTimer, longRemain.TotalMilliseconds);
            TimerHandler.RestartTimer(ref minuteTimer, HALF_MINUTE_IN_MS);

            SetTaskbarTooltip(TimeToNextLongBreak() + TOOLTIP_LONG_MSG); 
        }

        /// <summary>
        /// Resets the timer which generated the latest break event
        /// </summary>
        private void ResetElaspedTimer(ref TimeSpan longIntervalTime, ref TimeSpan shortIntervalTime)
        {
            SubstractForNotification(ref shortIntervalTime);

            if (isShortIntervalTimerDone)
            {
                TimerHandler.RestartTimer(ref shortIntervalTimer, shortIntervalTime.TotalMilliseconds);
                TimerHandler.ResetCounterTime(ref shortBreakLengthSoFar);

                isShortIntervalTimerDone = false;
            }

            if (isLongIntervalTimerDone)
            {
                SubstractForNotification(ref longIntervalTime);

                TimerHandler.RestartTimer(ref longIntervalTimer, longIntervalTime.TotalMilliseconds);
                TimerHandler.ResetCounterTime(ref longBreakLengthSoFar);

                // Restart the short timer too....
                if (settings.ShortIntervalTime > TimeSpan.Zero)
                {
                    TimerHandler.RestartTimer(ref shortIntervalTimer, shortIntervalTime.TotalMilliseconds);
                    TimerHandler.ResetCounterTime(ref shortBreakLengthSoFar);
                }

                isLongIntervalTimerDone = false;
            }

            // For "accuracy's" sake restart the minute timer too
            TimerHandler.RestartTimer(ref minuteTimer, HALF_MINUTE_IN_MS);
            
            // And refresh the toolbar tooltip
            SetTaskbarTooltip(TimeToNextLongBreak() + TOOLTIP_LONG_MSG);
        }

        /// <summary>
        /// Zeros out the counter whose timer is done
        /// </summary>
        private void ResetFinishedTimeCounter()
        {
            if (isShortIntervalTimerDone)
            {
                TimerHandler.ResetCounterTime(ref shortBreakLengthSoFar);
            }
            else if (isLongIntervalTimerDone)
            {
                TimerHandler.ResetCounterTime(ref longBreakLengthSoFar);
            }
        }

        /// <summary>
        /// Gives back the remaining minutes till the next long break
        /// </summary>
        private double TimeToNextLongBreak()
        {
            TimeSpan totalTime = settings.LongIntervalTime;
            TimeSpan remainingTime = totalTime - longBreakLengthSoFar;

            return Math.Round(remainingTime.TotalMinutes, 1);
        }

        /// <summary>
        /// Gives back the amount of minutes left until the end of the current pause state
        /// </summary>
        private double TimeToPauseEnd()
        {
            TimeSpan remainingTime = pauseTotalLength - pauseLengthSoFar;

            return Math.Round(remainingTime.TotalMinutes, 1);
        }

        /// <summary>
        /// If pre-break notification is enabled then the duration of the popup is substracted 
        /// from the time argument. 
        /// Usage: Call when giving value to the timers which govern the break times
        /// </summary>
        private void SubstractForNotification(ref TimeSpan time)
        {
            if (settings.IsNotificationEnabled && time > TimeSpan.Zero)
            {
                time -= settings.NotificationLength;
            }
        }

        /// <summary>
        /// Resets all the needed timers
        /// Usage: When the blocker window has closed, or when the break is postponed
        /// </summary>
        private void ResetTimers(TimeSpan longIntervalTime, TimeSpan shortIntervalTime)
        {
            // Order is important!
            ResetFinishedTimeCounter(); //Reset the related stopwatch too
            ResetElaspedTimer(ref longIntervalTime, ref shortIntervalTime); // Restart the clock that started the window that closed
            TimerHandler.RestartTimer(ref minuteTimer, HALF_MINUTE_IN_MS); // Reset the minute timer, so the counts are more precise
        }

        #endregion

        #region Taskbar manipulation
        /// <summary>
        /// Sets the taskbar tooltip to the given text
        /// </summary>
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
        private void DisableTaskbarOption(MenuItem option)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                option.IsEnabled = false;
            }));
        }

        /// <summary>
        /// Enable the given taskbar MenuItem
        /// </summary>
        private void EnableTaskbarOption(MenuItem option)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                option.IsEnabled = true;
            }));
        }

        /// <summary>
        /// Switch to 'Resume' btn on taskbar
        /// </summary>
        private void ActivateResumeBtn()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                PauseItem.Header = "Resume";
                PauseItem.Click -= PauseItem_Click;
                PauseItem.Click += ResumeItem_Click;
            }));
        }

        /// <summary>
        /// Switch to 'Pause' btn on taskbar
        /// </summary>
        private void ActivatePauseBtn()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                PauseItem.Header = "Pause";
                PauseItem.Click += PauseItem_Click;
                PauseItem.Click -= ResumeItem_Click;
            }));
        }
        #endregion

        #region Helpers
        /// <summary>
        /// Resets the postpone count which reached the limit set by the user
        /// </summary>
        private void ResetPostponeCount()
        {
            if (longBreakPostponeCount >= settings.PostponeAmount)
            {
                longBreakPostponeCount = 0;
            }

            if (shortBreakPostponeCount >= settings.PostponeAmount)
            {
                shortBreakPostponeCount = 0;
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
                    // Dispose of all the stuff that could remain in memory after exit
                    shortIntervalTimer.Dispose();
                    longIntervalTimer.Dispose();
                    minuteTimer.Dispose();
                    pauseTimer.Dispose();
                    taskbarIcon.Dispose();
                    keyTrap?.Dispose();

                    // Important! See the manual for the event
                    // SystemEvents.PowerModeChanged -= OnPowerChange;
                }

                // free unmanaged resources (unmanaged objects) and override a finalizer below.
                // set large fields to null.

                disposedValue = true;
            }
        }

        // override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~TaskbarPresence() {
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
