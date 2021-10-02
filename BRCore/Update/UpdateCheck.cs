using BRCore.Update.DTO;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BRCore.Update
{
    internal class UpdateCheck
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static readonly string PRODUCT_NAME = "BlinkReminder";
        private static readonly string RELEASES_URL = "https://api.github.com/repos/ZsoltGajdacs/BlinkReminder/releases";
        private static readonly string CHECK_FAILED = "Check failed:";
        private static readonly string PROTOCOL_ERROR = "Protocol Error";
        private static readonly string CONNECTION_ERROR = "Couldn't connect to GitHub";
        private static readonly string UNKNOWN_CONNECTION_ERROR = "Unknown connection error";
        private static readonly string API_ERROR = "Github API mismatch";
        private static readonly string NO_UPDATE = "No new version";

        private readonly VersionDto currentVersion;

        internal UpdateCheck(VersionDto currentVersion)
        {
            this.currentVersion = currentVersion;
        }

        internal async Task<UpdateResultDto> GetUpdateUrl()
        {
            HttpClient httpClient = new HttpClient();
            ProductInfoHeaderValue header = new ProductInfoHeaderValue(PRODUCT_NAME, currentVersion.VersionText);
            httpClient.DefaultRequestHeaders.UserAgent.Add(header);
            string content = String.Empty;

            try
            {
                content = await httpClient.GetStringAsync(RELEASES_URL);
            }
            catch (ProtocolViolationException e)
            {
                logger.Error(e, "Protocol error");
                return new UpdateResultDto(false, CHECK_FAILED + " " + PROTOCOL_ERROR, string.Empty);
            }
            catch (HttpRequestException e)
            {
                if (e.Message.Contains("40"))
                {
                    logger.Error(e, "API error");
                    return new UpdateResultDto(false, CHECK_FAILED + " " + API_ERROR, string.Empty);
                }
                else
                {
                    logger.Error(e, "Connection error");
                    return new UpdateResultDto(false, CHECK_FAILED + " " + CONNECTION_ERROR, string.Empty);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, UNKNOWN_CONNECTION_ERROR);
                return new UpdateResultDto(false, CHECK_FAILED + " " + UNKNOWN_CONNECTION_ERROR, string.Empty);
            }

            return ParseJsonForDownloadUrl(content);
        }

        /// <summary>
        /// Returns the download URL if it's newer than the current version
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        private UpdateResultDto ParseJsonForDownloadUrl(string jsonString)
        {
            try
            {
                /*JArray json = JArray.Parse(jsonString);
                dynamic jsonData = json[0];
                string tag = jsonData.tag_name;

                if (CheckIfVersionIsNewer(tag))
                {
                    dynamic assets = jsonData.assets;
                    return new UpdateResultDto(true, string.Empty, assets[0].browser_download_url);
                }
                else
                {
                    return new UpdateResultDto(false, NO_UPDATE, string.Empty);
                }*/
                return new UpdateResultDto(false, NO_UPDATE, string.Empty);
            }
            catch (Exception e)
            {
                logger.Error(e, API_ERROR);
                return new UpdateResultDto(false, CHECK_FAILED + " " + API_ERROR, string.Empty);
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
            if (int.Parse(gitVerArr[0]) > currentVersion.MajorVersion)
            {
                return true;
            }
            // Minor version comparison
            else if (int.Parse(gitVerArr[1]) > currentVersion.MinorVersion)
            {
                return true;
            }
            // Revision version comparison
            else if (int.Parse(gitVerArr[2]) > currentVersion.RevisionVersion)
            {
                return true;
            }
            // If neither is higher, there is no new version
            else
            {
                return false;
            }
        }
    }
}
