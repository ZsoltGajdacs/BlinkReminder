using BRCore;
using BRWPF.Controls;
using BRWPF.Utils;
using System;
using System.Windows;
using System.Windows.Controls;

namespace BRWPF.Windows
{
    public partial class TaskbarPresence : Window, IDisposable
    {
        #region Data members
        // Logger
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // Windows
        private Window settingsWindow;
        private Window blockerWindow;
        private Window aboutWindow;
        private PauseWindow pauseWindow;

        // Pop-ups
        private BreakNotificationPopup breakPopup;

        // Keyboard input catcher
        private KeyboardHook keyTrap;

        // Core router
        private IBRCoreRouter coreRouter;
        #endregion

        #region CTOR and init
        public TaskbarPresence()
        {
            PreInit();
            InitializeComponent();
            PostInit();
        }

        private void PreInit()
        {
            coreRouter = new BRCoreRouter();
            coreRouter.StartBreakEvent += CoreRouter_StartBreak;
            coreRouter.StopBreak += CoreRouter_StopBreak;
        }

        private void PostInit()
        {

        }
        #endregion

        #region Taskbar event handlers
        private void PauseItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
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

                int pauseAmount = pauseWindow.ShowDialog();
                coreRouter.PauseMeasurement(TimeSpan.FromMinutes(pauseAmount));
            }));
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (settingsWindow == null)
                {
                    settingsWindow = new SettingsWindow(coreRouter.GetSettings());
                    settingsWindow.Closed += SettingsWindow_Closed;

                    /*GeneralSettingsDto settingsDto = settingsWindow;
                    coreRouter.UpdateSettings(settingsDto);*/
                }
                else
                {
                    settingsWindow.Activate();
                }
            }));
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (aboutWindow == null)
                {
                    aboutWindow = new AboutWindow(coreRouter.GetUpdateRunner());
                    aboutWindow.Closed += AboutWindow_Closed;
                    aboutWindow.Show();
                }
                else
                {
                    aboutWindow.Activate();
                }
            }));
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ResumeItem_Click(object sender, RoutedEventArgs e)
        {
            ActivatePauseBtn();
            coreRouter.ResumeMeasurement();
        }
        #endregion

        #region Window handlers
        private void ShowViewBlocker(TimeSpan interval, double scaling, bool isSkippable,
            bool isFullscreen, bool isLongBreakLocksScreen, bool isLongBreak, string message)
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                if (blockerWindow == null)
                {
                    if (isFullscreen)
                    {
                        keyTrap = new KeyboardHook(); // Intercept every key
                    }
                    blockerWindow = new ViewBlockerWindow(interval, scaling, isSkippable, isFullscreen,
                                                            isLongBreakLocksScreen, isLongBreak, message);
                    blockerWindow.Closed += BlockerWindow_Closed;
                    blockerWindow.Show();

                }
                else { blockerWindow.Activate(); }
            }));
        }
        #endregion

        #region Window Event handlers
        private void BlockerWindow_Closed(object sender, EventArgs e)
        {
            blockerWindow = null;

            keyTrap?.Dispose(); // Release keyboard trap
            coreRouter.ResetMeasurement();

            // Set back the taskbarWindow to be the main one
            Application.Current.MainWindow = this;
        }

        private void SettingsWindow_Closed(object sender, EventArgs e)
        {
            settingsWindow = null;
            //Serializer.JsonObjectSerialize<UserSettings>(settings.SettingsDirPath, settings.SettingsFilePath, ref settings, DoBackup.Yes);
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
            /*bool isLongBreak = true;

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
            }*/
        }
        #endregion

        #region Break event handlers
        private void CoreRouter_StopBreak()
        {
            throw new NotImplementedException();
        }

        private void CoreRouter_StartBreak()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Popup handling
        /// <summary>
        /// Shows the pre-notification popup with the given text. PostponeCount is for the internal 
        /// postpone limiter
        /// </summary>
        private void ShowNotificationPopup(string popupText, int popupPostponeCount, int notificationLength)
        {
            breakPopup.SetValues(popupText, popupPostponeCount);
            taskbarIcon.ShowCustomBalloon(breakPopup,
                System.Windows.Controls.Primitives.PopupAnimation.Slide, notificationLength);
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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose of all the stuff that could remain in memory after exit
                    /*shortIntervalTimer.Dispose();
                    longIntervalTimer.Dispose();
                    minuteTimer.Dispose();
                    pauseTimer.Dispose();*/
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
