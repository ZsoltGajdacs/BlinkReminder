using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace BlinkReminder.Helpers
{
    internal class UpdateCheck
    {
        // Consts
        private static readonly string PRODUCT_NAME = "BlinkReminder";
        private static readonly string RELEASES_URL = "https://api.github.com/repos/ZsoltGajdacs/BlinkReminder/releases";
        private static readonly string CHECK_FAILED = "Update check failed:";
        private static readonly string PROTOCOL_ERROR = "Protocol Error";
        private static readonly string CONNECTION_ERROR = "No Internet connection";
        private static readonly string API_ERROR = "Github API mismatch";
        private static readonly string NO_UPDATE = "No new version";

        private readonly int[] currentVersionArr;

        internal UpdateCheck(ref int[] versionNums)
        {
            currentVersionArr = versionNums;
        }

        internal async Task<string> GetUpdateUrl(string version)
        {
            HttpClient httpClient = new HttpClient();
            ProductInfoHeaderValue header = new ProductInfoHeaderValue(PRODUCT_NAME, version);
            httpClient.DefaultRequestHeaders.UserAgent.Add(header);
            string content = String.Empty;

            try
            {
                content = await httpClient.GetStringAsync(RELEASES_URL);
            }
            catch (ProtocolViolationException)
            {
                // log
                return CHECK_FAILED + " " + PROTOCOL_ERROR;
            }
            catch (Exception)
            {
                //log
                return CHECK_FAILED + " " + CONNECTION_ERROR;
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
            catch (Exception)
            {
                //log
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
