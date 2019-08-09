using System;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace BlinkReminder.Helpers
{
    internal class UpdateCheck
    {
        // Logger
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // Consts
        private static readonly string PRODUCT_NAME = "BlinkReminder";
        private static readonly string RELEASES_URL = "https://api.github.com/repos/ZsoltGajdacs/BlinkReminder/releases";
        private static readonly string CHECK_FAILED = "Check failed:";
        private static readonly string PROTOCOL_ERROR = "Protocol Error";
        private static readonly string CONNECTION_ERROR = "Couldn't connect to GitHub";
        private static readonly string UNKNOWN_CONNECTION_ERROR = "Unknown connection error";
        private static readonly string API_ERROR = "Github API mismatch";
        private static readonly string NO_UPDATE = "No new version";

        private readonly int[] currentVersionArr;
        internal readonly string versionText;

        internal UpdateCheck(int[] versionNums)
        {
            currentVersionArr = versionNums;
            versionText = versionNums[0] + "." + versionNums[1] + "." + versionNums[2];
        }

        internal async Task<string> GetUpdateUrl()
        {
            HttpClient httpClient = new HttpClient();
            ProductInfoHeaderValue header = new ProductInfoHeaderValue(PRODUCT_NAME, versionText);
            httpClient.DefaultRequestHeaders.UserAgent.Add(header);
            string content = String.Empty;

            try
            {
                content = await httpClient.GetStringAsync(RELEASES_URL);
            }
            catch (ProtocolViolationException e)
            {
                logger.Error("Protocol error", e);
                return CHECK_FAILED + " " + PROTOCOL_ERROR;
            }
            catch (HttpRequestException e)
            {
                if (e.Message.Contains("40"))
                {
                    logger.Error("API error", e);
                    return CHECK_FAILED + " " + API_ERROR;
                }
                else
                {
                    logger.Error("Connection error", e);
                    return CHECK_FAILED + " " + CONNECTION_ERROR;
                }
            }
            catch (Exception e)
            {
                logger.Error(UNKNOWN_CONNECTION_ERROR, e);
                return CHECK_FAILED + " " + UNKNOWN_CONNECTION_ERROR;
            }

            return ParseJsonForDownloadUrl(content);
        }

        /// <summary>
        /// Returns the download URL if it's newer than the current version
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private string ParseJsonForDownloadUrl(string jsonString)
        {
            try
            {
                JArray json = JArray.Parse(jsonString);
                dynamic data = json[0];
                string tag = data.tag_name;

                if (CheckIfVersionIsNewer(tag))
                {
                    dynamic assets = data.assets;
                    return assets[0].browser_download_url;
                }
                else
                {
                    return NO_UPDATE;
                }
            }
            catch (Exception e)
            {
                logger.Error(API_ERROR, e);
                return CHECK_FAILED + " " + API_ERROR;
            }
        }

        /// <summary>
        /// Checks if the git version is higher than the local one
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private bool CheckIfVersionIsNewer(string tag)
        {
            // My version tag looks like this: vx.x.x
            string[] gitVerArr = tag.Substring(1).Split('.');

            // Major version comparison
            if (int.Parse (gitVerArr[0]) > currentVersionArr[0])
            {
                return true;
            }
            // Minor version comparison
            else if (int.Parse(gitVerArr[1]) > currentVersionArr[1])
            {
                return true;
            }
            // Revision version comparison
            else if (int.Parse(gitVerArr[2]) > currentVersionArr[2])
            {
                return true;
            }
            // Not newer...
            else
            {
                return false;
            }
        }
    }
}
