using BlinkReminder.Update;
using System;
using System.Windows;
using System.Windows.Documents;

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
        private static readonly string DOWNLOAD_LINK_TEXT = "Click to update";
        private static readonly string ISSUE_LINK_TEXT = "You can report bugs here (GitHub)";
        private static readonly string ISSUE_LINK_ADDRESS = "https://github.com/ZsoltGajdacs/BlinkReminder/issues/new/choose";

        private string updateLinkUrl;

        private UpdateCheck update;

        internal AboutWindow(ref UpdateCheck updater)
        {
            InitializeComponent();

            this.update = updater;
            updateLinkUrl = String.Empty;

            SetTexts(update.versionText);
            SetUpdateLabel(VERSION_CHECK_INIT_LABEL);
            CheckUpdate();
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
                Run linkText = new Run(DOWNLOAD_LINK_TEXT);
                Hyperlink updateLink = new Hyperlink(linkText);
                updateLink.Click += UpdateLabel_Click;
                updateLink.NavigateUri = new Uri(result);

                updateLabel.Content = updateLink;
                updateLinkUrl = result;
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

        private async void UpdateLabel_Click(object sender, RoutedEventArgs e)
        {
            if (updateLinkUrl != String.Empty)
            {
                UpdateHandler updater = new UpdateHandler();
                bool isOkToLaunch = await updater.DownloadUpdate(updateLinkUrl);

                if (isOkToLaunch)
                {
                    updater.RunUpdate();
                }

                Close();
            }
        }

        /// <summary>
        /// Opens the browser to go to link
        /// </summary>
        private void HyperLink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.ToString());
        }

    }
}
