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
        private static readonly string VERSION_LABEL = "Version:";
        private static readonly string VERSION_CHECK_INIT_LABEL = "Checking update....";
        private static readonly string DOWNLOAD_LINK_TEXT = "Click to download the update";
        private static readonly string ISSUE_LINK_TEXT = "You can report bugs here (GitHub)";
        private static readonly string ISSUE_LINK_ADDRESS = "https://github.com/ZsoltGajdacs/BlinkReminder/issues/new/choose";

        private UpdateCheck update;

        public AboutWindow(ref int[] versionNums)
        {
            InitializeComponent();

            update = new UpdateCheck(ref versionNums);
            string versionText = versionNums[0] + "." + versionNums[1] + "." + versionNums[2];

            SetTexts(ref versionText);
            CheckUpdate(versionText);
        }

        private void SetTexts(ref string versionText)
        {
            authorLabel.Content = AUTHOR_LABEL;
            versionLabel.Content = VERSION_LABEL + " " + versionText;
            updateLabel.Content = VERSION_CHECK_INIT_LABEL;

            Run linkText = new Run(ISSUE_LINK_TEXT);
            Hyperlink issueLink = new Hyperlink(linkText);
            issueLink.RequestNavigate += HyperLink_RequestNavigate;
            issueLink.NavigateUri = new Uri(ISSUE_LINK_ADDRESS);
            issueLabel.Content = issueLink;
        }

        private async void CheckUpdate(string versionText)
        {
            string result = await update.GetUpdateUrl(versionText);

            if (result.StartsWith("https"))
            {
                Run linkName = new Run(DOWNLOAD_LINK_TEXT);
                Hyperlink downloadLink = new Hyperlink(linkName);
                downloadLink.RequestNavigate += HyperLink_RequestNavigate;
                downloadLink.NavigateUri = new Uri(result);
                
                updateLabel.Content = downloadLink;
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

        private void HyperLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
