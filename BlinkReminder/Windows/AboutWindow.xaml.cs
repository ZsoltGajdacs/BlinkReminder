using BlinkReminder.Helpers;
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
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        // Consts
        private static readonly string AUTHOR_LABEL = "Author: Zsolt Gajdács";
        private static readonly string VERSION_LABEL = "Current version:";
        private static readonly string VERSION_CHECK_INIT_LABEL = "Checking update....";
        private static readonly string DOWNLOAD_LINK_TEXT = "Click to download the new version's installer";

        private UpdateCheck update;

        public AboutWindow(ref int[] versionNums)
        {
            InitializeComponent();

            update = new UpdateCheck(ref versionNums);
            string versionText = versionNums[0] + "." + versionNums[1] + "." + versionNums[2];

            authorLabel.Content = AUTHOR_LABEL;
            versionLabel.Content = VERSION_LABEL + " " + versionText;
            updateLabel.Content = VERSION_CHECK_INIT_LABEL;

            CheckUpdate(versionText);
        }

        private async void CheckUpdate(string versionText)
        {
            string result = await update.GetUpdateUrl(versionText);

            if (result.StartsWith("https"))
            {
                Run linkName = new Run(DOWNLOAD_LINK_TEXT);
                Hyperlink downloadLink = new Hyperlink(linkName);
                downloadLink.NavigateUri = new Uri(result);
                
                updateLabel.Content = downloadLink;
                updateLabel.HorizontalAlignment = HorizontalAlignment.Center;
            }
            else
            {
                updateLabel.Content = result;
            }
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
