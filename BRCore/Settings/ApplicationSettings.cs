using BRCore.MeasurementSystems;
using BRCore.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BRCore.Settings
{
    /// <summary>
    /// Keeps the current settings
    /// </summary>
    [Serializable]
    internal sealed class ApplicationSettings
    {
        #region Data members
        internal string SettingsFilePath { get; private set; }
        internal string SettingsDirPath { get; private set; }

        public bool ShouldBreakWhenFullScreenAppDetected { get; set; }

        public bool IndefPauseEnabled { get; set; }
        public int DefaultPauseLength { get; set; }

        public bool IsNotificationEnabled { get; set; }
        public bool IsPermissiveNotification { get; set; }

        public List<ScreenBreak> Breaks { get; private set; }
        #endregion

        #region CTORs
        [JsonConstructor]
        private ApplicationSettings(string settingsFilePath, string settingsDirPath, bool shouldBreakWhenFullScreenAppDetected,
                                   bool indefPauseEnabled, int defaultPauseLength, bool isNotificationEnabled,
                                   bool isPermissiveNotification, List<ScreenBreak> breaks)
        {
            SettingsFilePath = settingsFilePath ?? throw new ArgumentNullException(nameof(settingsFilePath));
            SettingsDirPath = settingsDirPath ?? throw new ArgumentNullException(nameof(settingsDirPath));
            ShouldBreakWhenFullScreenAppDetected = shouldBreakWhenFullScreenAppDetected;
            IndefPauseEnabled = indefPauseEnabled;
            DefaultPauseLength = defaultPauseLength;
            IsNotificationEnabled = isNotificationEnabled;
            IsPermissiveNotification = isPermissiveNotification;
            Breaks = breaks ?? throw new ArgumentNullException(nameof(breaks));
        }

        /// <summary>
        /// Default ctor sets default values, called when there are no serialized settings
        /// </summary>
        public ApplicationSettings()
        {
            SetDefaults();
            Breaks = AddDefaultBreak();
        }
        #endregion

        #region Defaults
        private void SetDefaults()
        {
            SettingsDirPath = FilesLocation.GetSaveDirPath();
            SettingsFilePath = FilesLocation.GetSavePath();

            ShouldBreakWhenFullScreenAppDetected = true;
            IndefPauseEnabled = false;

            IsNotificationEnabled = true;
            IsPermissiveNotification = false;

            DefaultPauseLength = 30;
        }

        private List<ScreenBreak> AddDefaultBreak()
        {
            int breakNumericId = 1;
            BreakSettings defaultBreakSettings = new BreakSettings()
            {
                BreakInterval = TimeSpan.FromMinutes(10),
                BreakLength = TimeSpan.FromSeconds(10),
                IsSkippable = false,
                IsFullScreenBreak = true,
                PostponeAmount = 0,
                PostponeLength = TimeSpan.Zero,
                PreBreakNotificationLength = TimeSpan.FromSeconds(6),
                SmallBreakWindowScalingFactor = 1,
                BreakQuotes = new List<Quote>()
                {
                    new Quote("Look out the window", true, breakNumericId),
                    new Quote("Stretch your legs", true, breakNumericId),
                    new Quote("Close your eyes", true, breakNumericId),
                    new Quote("Drink some water", true, breakNumericId)
                }
            };

            ScreenBreak screenBreak = new ScreenBreak(breakNumericId, "Short Break", defaultBreakSettings, MeasurementType.TIMER);

            return new List<ScreenBreak> { screenBreak };
        }
        #endregion
    }
}
