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

namespace BlinkReminder.Windows
{
    /// <summary>
    /// Main window of application, actual window is hidden,
    /// only the taskbar icon is active
    /// </summary>
    public partial class TaskbarPresence : Window, IDisposable
    {
        private System.ComponentModel.IContainer components;
        private KeyboardHook kh;

        // Windows
        private Window settingsWindow;
        private Window longBlockerWindow;
        private Window shortBlockerWindow;

        //Timers
        private Timer shortCycleTimer;
        private Timer longCycleTimer;

        private UserSettings userSettings;


        public TaskbarPresence()
        {
            InitializeComponent();
            userSettings = UserSettings.Instance;
            components = new System.ComponentModel.Container();

            StartTimers();
        }

        #region Click Events

        private void ShortBreak_Click(object sender, RoutedEventArgs e)
        {
            ShowViewBlockerShort(userSettings.ShortDisplayTime);
        }

        private void LongBreak_Click(object sender, RoutedEventArgs e)
        {
            ShowViewBlockerLong(userSettings.LongDisplayTime);
        }

        private void PauseItem_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void ExitItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        #endregion

        #region Timer Events

        private void LongCycleTimer_Elapsed(object sender, EventArgs e)
        {
            ShowViewBlockerLong(userSettings.LongDisplayTime);
        }

        private void ShortCycleTimer_Elapsed(object sender, EventArgs e)
        {
            ShowViewBlockerShort(userSettings.ShortDisplayTime);
        }

        #endregion

        #region Window Events

        private void BlockerWindow_Closed(object sender, EventArgs e)
        {
            longBlockerWindow = null;
            shortBlockerWindow = null;
            kh.Dispose(); // Release keyboard trap
        }

        private void SettingsWindow_Closed(object sender, EventArgs e)
        {
            settingsWindow = null;
        }

        #endregion

        #region Window showers

        private void ShowViewBlockerLong(long interval)
        {
            if (longBlockerWindow == null)
            {
                kh = new KeyboardHook(); // Intercept every key
                longBlockerWindow = new ViewBlockerLong(interval);
                longBlockerWindow.Closed += BlockerWindow_Closed;
                longBlockerWindow.Show();
            }
            else { longBlockerWindow.Activate(); }
        }

        private void ShowViewBlockerShort(long interval)
        {
            if (shortBlockerWindow == null)
            {
                kh = new KeyboardHook(); // Intercept every key

                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    shortBlockerWindow = new ViewBlockerShort(interval);
                    shortBlockerWindow.Closed += BlockerWindow_Closed;
                    shortBlockerWindow.Show();
                }));
            }
            else { shortBlockerWindow.Activate(); }
        }

        private void ShowSettings()
        {
            if (settingsWindow == null)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    settingsWindow = new SettingsWindow();
                    settingsWindow.Closed += SettingsWindow_Closed;
                    settingsWindow.Show();
                }));
            }
            else { settingsWindow.Activate(); }
        }

        #endregion

        #region Helpers

        private void StartTimers()
        {
            shortCycleTimer = new Timer(userSettings.ShortIntervalTime)
            {
                AutoReset = true
            };
            shortCycleTimer.Elapsed += ShortCycleTimer_Elapsed;
            shortCycleTimer.Start();

            longCycleTimer = new Timer(userSettings.LongIntervalTime)
            {
                AutoReset = true
            };
            longCycleTimer.Elapsed += LongCycleTimer_Elapsed;
            longCycleTimer.Start();

            components.Add(shortCycleTimer);
            components.Add(longCycleTimer);
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
