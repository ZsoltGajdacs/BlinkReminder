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

        internal AboutWindow(ref UpdateCheck updater, string downloadUri)
        {
            InitializeComponent();

            this.update = updater;

            SetTexts(update.versionText);
            if (downloadUri.Equals(""))
            {
                SetUpdateLabel(VERSION_CHECK_INIT_LABEL);
                CheckUpdate();
            }
            else
            {
                SetUpdateLabel(downloadUri);
            }
        }

        /// <summary>
        /// Sets the constant texts - those known right at window start
        /// </summary>
        private void SetTexts(string versionText)
        {
            authorLabel.Content = AUTHOR_LABEL;
            versionLabel.Content = VERSION_LABEL + " " + versionText;

            Run linkText = new Run(ISSUE_LINK_TEXT);
            Hyperlink issueLink = new Hyperlink(linkText);
            issueLink.RequestNavigate += HyperLink_RequestNavigate;
            issueLink.NavigateUri = new Uri(ISSUE_LINK_ADDRESS);
            issueLabel.Content = issueLink;
        }

        /// <summary>
        /// Sets the label as a string or as a link based on the string supplied
        /// </summary>
        private void SetUpdateLabel(string result)
        {
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

        private async void CheckUpdate()
        {
            string result = await update.GetUpdateUrl();
            SetUpdateLabel(result);
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Opens the browser to download the newest release
        /// </summary>
        private void HyperLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }
    }
}
