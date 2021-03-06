﻿using BlinkReminder.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BlinkReminder.Update
{
    internal class UpdateHandler
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const string UPDATE_ZIPNAME = "brupdate.zip";
        private const string UPDATE_FILENAME = "BRSetup.msi";
        private const string UPDATE_CHANGELOG = "Changelog.txt";

        private readonly string updateZipFilePath;
        private readonly string updateFilePath;
        private readonly string updateChangelogPath;

        private UserSettings settings;

        public UpdateHandler()
        {
            settings = UserSettings.Instance;
            updateZipFilePath = settings.SettingsDirPath + "\\" + UPDATE_ZIPNAME;
            updateFilePath = settings.SettingsDirPath + "\\" + UPDATE_FILENAME;
            updateChangelogPath = settings.SettingsDirPath + "\\" + UPDATE_CHANGELOG;
        }

        internal async Task<bool> DownloadUpdate(string url)
        {
            CleanLastUpdate();

            using (var client = new WebClient())
            {
                await client.DownloadFileTaskAsync(new Uri(url), updateZipFilePath);
            }

            if (File.Exists(updateZipFilePath))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void RunUpdate()
        {
            try
            {
                ZipFile.ExtractToDirectory(updateZipFilePath, settings.SettingsDirPath);
            }
            catch (Exception e)
            {
                logger.Error("Update extraction failed", e);
            }

            if (File.Exists(updateFilePath)) Process.Start(updateFilePath);
        }

        /// <summary>
        /// Deletes the files downloaded for update
        /// </summary>
        internal void CleanLastUpdate()
        {
            if (File.Exists(updateZipFilePath))
            {
                File.Delete(updateZipFilePath);
            }

            if (File.Exists(updateFilePath))
            {
                File.Delete(updateFilePath);
            }

            if (File.Exists(updateChangelogPath))
            {
                File.Delete(updateChangelogPath);
            }
        }
        
    }
}
