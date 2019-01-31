using BlinkReminder.Settings;
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

        public SettingsWindow(ref UserSettings settings)
        {
            InitializeComponent();
            this.settings = settings;

            ShortQuoteInput.Text = QUOTE_INPUT_PLACEHOLDER;
            LongQuoteInput.Text = QUOTE_INPUT_PLACEHOLDER;

            SetDataBinding();
        }

        private void SetDataBinding()
        {
            SettingsGrid.DataContext = settings;
            ShortQuoteItems.ItemsSource = settings.ShortBreakQuotes;
            LongQuoteItems.ItemsSource = settings.LongBreakQuotes;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #region Quote Input Key events

        private void ShortQuoteInput_KeyDown(object sender, KeyEventArgs e)
        {
            // If this is the first user char then delete the placeholder
            if (ShortQuoteInput.Text.Equals(QUOTE_INPUT_PLACEHOLDER) && e.Key != Key.Enter)
            {
                ShortQuoteInput.Text = String.Empty;
            }
        }

        private void ShortQuoteInput_KeyUp(object sender, KeyEventArgs e)
        {
            // If ENTER is pressed add to list in settings
            if (e.Key == Key.Enter)
            {
                if (!ShortQuoteInput.Text.Equals(QUOTE_INPUT_PLACEHOLDER))
                {
                    settings.ShortBreakQuotes.Add(new Quote(ShortQuoteInput.Text, true, settings.ShortBreakQuotes.Count));
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
            // If this is the first user char then delete the placeholder
            if (LongQuoteInput.Text.Equals(QUOTE_INPUT_PLACEHOLDER) && e.Key != Key.Enter)
            {
                LongQuoteInput.Text = String.Empty;
            }
        }

        private void LongQuoteInput_KeyUp(object sender, KeyEventArgs e)
        {
            // If ENTER is pressed add to list in settings
            if (e.Key == Key.Enter)
            {
                if (!LongQuoteInput.Text.Equals(QUOTE_INPUT_PLACEHOLDER))
                {
                    settings.LongBreakQuotes.Add(new Quote(LongQuoteInput.Text, true, settings.LongBreakQuotes.Count));
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
    }
}
