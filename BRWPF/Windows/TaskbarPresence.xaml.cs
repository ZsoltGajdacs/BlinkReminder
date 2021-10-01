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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BRWPF.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TaskbarPresence : Window
    {
        #region Data members
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Window settingsWindow;
        private Window blockerWindow;
        private Window aboutWindow;
        private PauseWindow pauseWindow;
        #endregion

        public TaskbarPresence()
        {
            InitializeComponent();
        }

        #region Taskbar event handlers
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

        private void ResumeItem_Click(object sender, RoutedEventArgs e)
        {
            ActivatePauseBtn();
            ResumeTimers();
        }

        #region Window handlers
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
    }
}
