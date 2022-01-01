using BRCore.MeasurementSystems.TimerBasedMeasurement.Settings;
using BRCore.Settings.DTO;
using BRWPF.Mappers;
using BRWPF.Utils;
using BRWPF.Windows.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using ZsGUtils.Keys;

namespace BRWPF.Windows
{
    public partial class SettingsWindow : Window
    {
        private readonly Dictionary<KeyPair<int, string>, BreakTimerSettings> breakSettingsVMs;
        private GeneralSettingsViewModel generalSettingsVM;

        private readonly TooltipHandler tooltipHandler;

        private bool shouldSave;

        public SettingsWindow(SettingsDto settingsDto)
        {
            InitializeComponent();

            breakSettingsVMs = new Dictionary<KeyPair<int, string>, BreakTimerSettings>();
            tooltipHandler = new TooltipHandler();

            FillViewModels(settingsDto);
            SetDataBinding();
            SubscribeToEvents();
            SetControlAccessability();
        }

        public GeneralSettingsDto ShowCustomDialog()
        {
            ShowDialog();

            if (shouldSave)
            {
                return SettingsMapper.Instance.ToGeneralSettingsDto(generalSettingsVM);
            }
            else
            {
                return null;
            }
        }

        #region Startup methods
        /// <summary>
        /// Fills up all the viewmodels from the incoming settings data
        /// </summary>
        private void FillViewModels(SettingsDto settingsDto)
        {
            generalSettingsVM = SettingsMapper.Instance.ToGeneralSettingsViewModel(settingsDto.GeneralSettingsDto);
            // TODO: do break tab and fill it's model
        }

        /// <summary>
        /// Sets the panel data bindings
        /// </summary>
        private void SetDataBinding()
        {
            SettingsPanel.DataContext = generalSettingsVM;
        }

        /// <summary>
        /// Subscribes to VM events
        /// </summary>
        private void SubscribeToEvents()
        {
            generalSettingsVM.PropertyChanged += GeneralSettings_PropertyChanged;
        }
        #endregion

        #region Click Events
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            shouldSave = true;
            Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NewBreakTab_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        /// <summary>
        /// Deletes the Quote clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShortQuoteClick(object sender, MouseButtonEventArgs e)
        {
            /*UserSettings settings = UserSettings.Instance;

            // img's tag contains the quote's text
            Image img = sender as Image;

            Quote quoteToRemove = settings.ShortBreakQuotes.Where(i => i.QuoteText.Equals((string)img.Tag)).Single();
            settings.ShortBreakQuotes.Remove(quoteToRemove);*/
        }

        /// <summary>
        /// Deletes the Quote clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LongQuoteClick(object sender, MouseButtonEventArgs e)
        {
            /*UserSettings settings = UserSettings.Instance;

            // img's tag contains the quote's text
            Image img = sender as Image;

            Quote quoteToRemove = settings.LongBreakQuotes.Where(i => i.QuoteText.Equals((string)img.Tag)).Single();
            settings.LongBreakQuotes.Remove(quoteToRemove);*/
        }
        #endregion

        #region Property Events
        private void GeneralSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsFullscreenBreak"))
            {
                SetControlAccessability();
            }
        }

        private void SetControlAccessability()
        {
            if (generalSettingsVM.IsFullscreenBreak)
            {
                scaleControl.IsEnabled = false;
            }
            else
            {
                scaleControl.IsEnabled = true;
            }
        }
        #endregion
    }
}
