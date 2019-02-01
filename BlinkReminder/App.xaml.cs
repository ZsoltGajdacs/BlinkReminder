using BlinkReminder.Settings;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace BlinkReminder
{
    public partial class App : Application
    {
        /// <summary>
        /// Deletes the Quote clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShortQuoteClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UserSettings settings = UserSettings.Instance;

            // img's tag contains the quote's text
            Image img = sender as Image;

            Quote quoteToRemove = settings.ShortBreakQuotes.Where(i => i.QuoteText.Equals((string)img.Tag)).Single();
            settings.ShortBreakQuotes.Remove(quoteToRemove);
        }

        /// <summary>
        /// Deletes the Quote clicked on
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LongQuoteClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UserSettings settings = UserSettings.Instance;
            
            // img's tag contains the quote's text
            Image img = sender as Image;

            Quote quoteToRemove = settings.LongBreakQuotes.Where(i => i.QuoteText.Equals((string)img.Tag)).Single();
            settings.LongBreakQuotes.Remove(quoteToRemove);
        }
    }
}
