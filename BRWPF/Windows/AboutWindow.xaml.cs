using BRCore.Update;
using BRCore.Update.DTO;
using System;
using System.Windows;
using System.Windows.Documents;

namespace BRWPF.Windows
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
        private static readonly string DOWNLOAD_IN_PROGRESS = "Downloading update....";
        private static readonly string ISSUE_LINK_TEXT = "You can report bugs here (GitHub)";
        private static readonly string ISSUE_LINK_ADDRESS = "https://github.com/ZsoltGajdacs/BlinkReminder/issues/new/choose";

        private string updateLinkUrl;

        private UpdateRunner updater;

        internal AboutWindow(UpdateRunner updateRunner)
        {
            InitializeComponent();

            this.updater = updateRunner;
            updateLinkUrl = String.Empty;

            SetTexts(updater.CurrentVersion.VersionText);
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

            Hyperlink issueLink = new Hyperlink(new Run(ISSUE_LINK_TEXT));
            issueLink.RequestNavigate += HyperLink_RequestNavigate;
            issueLink.NavigateUri = new Uri(ISSUE_LINK_ADDRESS);
            issueLabel.Content = issueLink;
        }

        private async void CheckUpdate()
        {
            UpdateResultDto result = await updater.CheckUpdate();
            SetUpdateLabel(result.IsSuccessfulUpdateCheck ? result.UpdateLink : result.ErrorMessage);
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

        #region Click event handlers
        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private async void UpdateLabel_Click(object sender, RoutedEventArgs e)
        {
            if (updateLinkUrl != String.Empty)
            {
                updateLabel.Content = DOWNLOAD_IN_PROGRESS;
                await updater.RunUpdate(updateLinkUrl);

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
        #endregion
    }
}
