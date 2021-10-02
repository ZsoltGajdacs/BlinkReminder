using BRCore.MeasurementSystems.TimerBasedMeasurement.Settings;
using BRCore.Utils;
using NLog;
using NLog.Layouts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Timers;
using ZsGUtils.FilesystemUtils;
using ZsGUtils.Randoms;

namespace BRCore.Settings
{
    /// <summary>
    /// Keeps the current settings
    /// </summary>
    [Serializable]
    public sealed class UserSettings
    {
        #region Data members
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        internal string SettingsFilePath { get; private set; }
        internal string SettingsDirPath { get; private set; }

        public bool IsTimerMode { get; set; }
        public bool IsActivityMode { get; set; }

        public bool ShouldBreakWhenFullScreenAppDetected { get; set; }

        public bool IndefPauseEnabled { get; set; }
        public int DefaultPauseLength { get; set; }

        public bool IsNotificationEnabled { get; set; }
        public bool IsPermissiveNotification { get; set; }

        public List<BreakTimerSettings> BreakTimerSettings { get; private set; }

        #endregion

        #region Singleton stuff
        private static readonly Lazy<UserSettings> lazy = new Lazy<UserSettings>(() =>
        {
            // Configure logger
            NLog.Config.LoggingConfiguration config = new NLog.Config.LoggingConfiguration();
            NLog.Targets.FileTarget logfile = new NLog.Targets.FileTarget("logfile")
            {
                FileName = FilesLocation.GetSaveDirPath() + "\\brlog.log",
                Layout = new SimpleLayout("${longdate}|${level:uppercase=true}|${logger}|${threadid}|${message}|${exception:format=tostring}"),
                ArchiveOldFileOnStartup = true,
                MaxArchiveFiles = 5,
                ArchiveNumbering = NLog.Targets.ArchiveNumberingMode.DateAndSequence
            };
            config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
            LogManager.Configuration = config;

            // Deserialize settings
            UserSettings settings = JsonSerializer.Deserialize<UserSettings>(FilesLocation.GetSavePath());
            
            return settings ?? new UserSettings();
        });

        public static UserSettings Instance { get { return lazy.Value; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Default ctor sets default values, called when there are no serialized settings
        /// </summary>
        private UserSettings()
        {
            SettingsDirPath = FilesLocation.GetSaveDirPath();
            SettingsFilePath = FilesLocation.GetSavePath();

            ShouldBreakWhenFullScreenAppDetected = true;
            IndefPauseEnabled = false;

            IsNotificationEnabled = true;
            IsPermissiveNotification = false;

            logger.Info("New settings created with defaults");
        }
        #endregion

        #region Startup methods


        #endregion
    }
}
