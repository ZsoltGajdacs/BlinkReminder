using BlinkReminder.Helpers;
using BlinkReminder.Settings;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace BlinkReminder.Windows
{
    /// <summary>
    /// Code behind for the settings window
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private static string QUOTE_INPUT_PLACEHOLDER = "Add new quote here";

        private UserSettings settings;
        private TooltipHandler tooltipHandler;

        public SettingsWindow(ref UserSettings settings)
        {
            InitializeComponent();
            this.settings = settings;
            tooltipHandler = new TooltipHandler();

            SetDefaults();
            SetDataBinding();
        }

        #region Startup methods
        private void SetDataBinding()
        {
            SettingsGrid.DataContext = settings;
            ShortQuoteItems.ItemsSource = settings.ShortBreakQuotes;
            LongQuoteItems.ItemsSource = settings.LongBreakQuotes;
        }

        private void SetDefaults()
        {
            ShortQuoteInput.Text = QUOTE_INPUT_PLACEHOLDER;
            LongQuoteInput.Text = QUOTE_INPUT_PLACEHOLDER;
        }
        #endregion

        #region Click Events
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
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

                if (quote.IsShort)
                {
                    tooltipHandler.ShowTooltipOnTextBox(ref ShortQuoteInput, quoteMsg);
                }
                else
                {
                    tooltipHandler.ShowTooltipOnTextBox(ref LongQuoteInput, quoteMsg);
                }
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

        #region Input check of display/interval values
        private void TimesTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !TextValidator.IsNumsOnly(e.Text);
        }

        private void TimesTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox.Text.Equals("0"))
            {
                textBox.Text = null;
                tooltipHandler.ShowTooltipOnTextBox(ref textBox, "Can't have 0 seconds!");
            }
        }
        #endregion
    }
}
