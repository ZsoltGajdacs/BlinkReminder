using BRCore.Settings;
using BRCore.Settings.DTO;
using BRWPF.Utils;
using BRWPF.Windows.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BRWPF.Windows
{
    public partial class SettingsWindow : Window
    {
        private static string QUOTE_INPUT_PLACEHOLDER = "Add new quote here";

        private readonly Dictionary<string, SettingsBreakViewModel> breakSettingsVMs;
        private GeneralSettingsViewModel generalSettingsVM;

        private readonly TooltipHandler tooltipHandler;

        private bool shouldSave;

        public SettingsWindow(SettingsDto settingsDto)
        {
            InitializeComponent();

            breakSettingsVMs = new Dictionary<string, SettingsBreakViewModel>();
            tooltipHandler = new TooltipHandler();

            FillViewModels(settingsDto);
            SetDataBinding();
            SubscribeToEvents();
            CheckDisabledState();
            SetControlAccessability();
        }

        public new SettingsDto ShowDialog()
        {
            base.ShowDialog();

            if (shouldSave)
            {
                throw new NotImplementedException();
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        #region Startup methods
        /// <summary>
        /// Fills up all the viewmodels from the incoming settings data
        /// </summary>
        private void FillViewModels(SettingsDto settingsDto)
        {

        }

        /// <summary>
        /// Sets the panel data bindings
        /// </summary>
        private void SetDataBinding()
        {
            SettingsPanel.DataContext = generalSettingsVM;
        }

        /// <summary>
        /// Subscribes to settings events
        /// </summary>
        private void SubscribeToEvents()
        {
            settings.PropertyChanged += Settings_PropertyChanged;
            settings.SettingsDTO.PropertyChanged += SettingsDTO_PropertyChanged;
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
        #endregion

        #region Property Events
        private void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("IsFullscreenBreak"))
            {
                SetControlAccessability();
            }
        }

        private void SettingsDTO_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            CheckDisabledState();
        }
        #endregion

        #region Property helpers
        /// <summary>
        /// Checks if the timers are disabled in the settings obj
        /// </summary>
        private void CheckDisabledState()
        {
            if (settings.LongIntervalTime == TimeSpan.Zero)
            {
                longDispControl.IsEnabled = false;
            }

            if (settings.ShortIntervalTime == TimeSpan.Zero)
            {
                shortDispControl.IsEnabled = false;
            }
        }

        /// <summary>
        /// Sets the read/write state of UI controls
        /// </summary>
        private void SetControlAccessability()
        {
            if (settings.IsFullscreenBreak)
            {
                scaleControl.IsEnabled = false;
            }
            else
            {
                scaleControl.IsEnabled = true;
            }

        }
        #endregion

        #region Quote Input Key events

        private void ShortQuoteInput_KeyDown(object sender, KeyEventArgs e)
        {
            ClearPlaceholder(ref ShortQuoteInput, ref e);
        }

        private void ShortQuoteInput_KeyUp(object sender, KeyEventArgs e)
        {
            // If ENTER is pressed add to list in settings
            if (e.Key == Key.Enter)
            {
                if (!ShortQuoteInput.Text.Equals(QUOTE_INPUT_PLACEHOLDER))
                {
                    AddNewQuote(settings.ShortBreakQuotes, ShortQuoteInput.Text, true);
                    ShortQuoteInput.Text = QUOTE_INPUT_PLACEHOLDER;
                }
            }

            // If the input is empty add placeholder
            if (ShortQuoteInput.Text.Equals(String.Empty))
            {
                ShortQuoteInput.Text = QUOTE_INPUT_PLACEHOLDER;
            }
        }

        private void LongQuoteInput_KeyDown(object sender, KeyEventArgs e)
        {
            ClearPlaceholder(ref LongQuoteInput, ref e);
        }

        private void LongQuoteInput_KeyUp(object sender, KeyEventArgs e)
        {
            // If ENTER is pressed add to list in settings
            if (e.Key == Key.Enter)
            {
                if (!LongQuoteInput.Text.Equals(QUOTE_INPUT_PLACEHOLDER))
                {
                    AddNewQuote(settings.LongBreakQuotes, LongQuoteInput.Text, false);
                    LongQuoteInput.Text = QUOTE_INPUT_PLACEHOLDER;
                }
            }

            // If the input is empty add placeholder
            if (LongQuoteInput.Text.Equals(String.Empty))
            {
                LongQuoteInput.Text = QUOTE_INPUT_PLACEHOLDER;
            }
        }
        #endregion

        #region Quote Support
        /// <summary>
        /// Adds a new quote the passed list if it doesn't already exist
        /// </summary>
        /// <param name="quoteList"></param>
        /// <param name="quoteToAdd"></param>
        /// <param name="isShort"></param>
        private void AddNewQuote(BindingList<Quote> quoteList, string quoteToAdd, bool isShort)
        {
            Quote quote = quoteList.Where(q => q.QuoteText.Equals(quoteToAdd)).SingleOrDefault();

            if (quote == null)
            {
                quoteList.Add(new Quote(quoteToAdd, true, isShort));
            }
            else
            {
                string quoteMsg = "Quote already added";

                Control control = quote.IsShort ? ShortQuoteInput : LongQuoteInput;
                tooltipHandler.ShowTooltipOnTextBox(ref control, quoteMsg);
            }
        }

        /// <summary>
        /// Clears the placeholder for the passed textbox if the placeholder text is shown
        /// </summary>
        /// <param name="textBox"></param>
        /// <param name="e"></param>
        private void ClearPlaceholder(ref TextBox textBox, ref KeyEventArgs e)
        {
            if (textBox.Text.Equals(QUOTE_INPUT_PLACEHOLDER) && e.Key != Key.Enter)
            {
                textBox.Text = String.Empty;
            }
        }
        #endregion

        #region Timer disable support

        private void ShortIntervalControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (shortIntervalControl.Value == 0)
            {
                shortDispControl.IsEnabled = false;
            }
            else
            {
                shortDispControl.IsEnabled = true;
            }
        }

        private void LongIntervalControl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (longIntervalControl.Value == 0)
            {
                longDispControl.IsEnabled = false;
            }
            else
            {
                longDispControl.IsEnabled = true;
            }
        }

        #endregion
    }
}
