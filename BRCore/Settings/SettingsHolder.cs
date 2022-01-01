using BRCore.Utils;
using NLog;
using NLog.Layouts;
using System;
using ZsGUtils.FilesystemUtils;

namespace BRCore.Settings
{
    internal class SettingsHolder
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        internal ApplicationSettings Settings { get; private set; }

        #region CTORs
        private SettingsHolder(ApplicationSettings settings)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        private SettingsHolder()
        {
            Settings = new ApplicationSettings();
            logger.Info("New settings created with defaults");
        }
        #endregion

        #region Singleton stuff
        public static SettingsHolder Instance => lazy.Value;

        private static readonly Lazy<SettingsHolder> lazy = new Lazy<SettingsHolder>(() =>
        {
            ConfigureLogger();

            ApplicationSettings settings = JsonSerializer.Deserialize<ApplicationSettings>(FilesLocation.GetSavePath());

            return settings != null ? new SettingsHolder(settings)
                                    : new SettingsHolder();
        });
        #endregion

        private static void ConfigureLogger()
        {
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
        }
    }
}
