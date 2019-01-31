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
        private void ShortQuoteClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UserSettings settings = UserSettings.Instance;
            
            // img's tag is the index of the list where the Quote is kept
            Image img = sender as Image;

            settings.ShortBreakQuotes.RemoveAt((int)img.Tag);
        }

        private void LongQuoteClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            UserSettings settings = UserSettings.Instance;
            
            // img's tag is the index of the list where the Quote is kept
            Image img = sender as Image;

            settings.LongBreakQuotes.RemoveAt((int)img.Tag);
        }
    }
}
