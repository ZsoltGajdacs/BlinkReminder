using BRCore.MeasurementSystems;
using BRCore.Settings;
using BRCore.Settings.DTO;
using System.Collections.Generic;

namespace BRCore.Mappers
{
    internal static class SettingsMapper
    {
        public static SettingsDto SettingsToDto(ApplicationSettings appSettings)
        {
            return new SettingsDto(GeneralSettingsToDto(appSettings), BreaksToDto(appSettings));
        }

        public static GeneralSettingsDto GeneralSettingsToDto(ApplicationSettings appSettings)
        {
            return new GeneralSettingsDto()
            {
                DefaultPauseLength = appSettings.DefaultPauseLength,
                IndefPauseEnabled = appSettings.IndefPauseEnabled,
                IsNotificationEnabled = appSettings.IsNotificationEnabled,
                IsPermissiveNotification = appSettings.IsPermissiveNotification,
                ShouldBreakWhenFullScreenAppDetected = appSettings.ShouldBreakWhenFullScreenAppDetected
            };
        }

        public static List<BreakDto> BreaksToDto(ApplicationSettings appSettings)
        {
            GeneralSettingsDto generalSettingsDto = GeneralSettingsToDto(appSettings);
            List<BreakDto> breakDtos = new List<BreakDto>();

            foreach (var screenBreak in appSettings.Breaks)
            {
                breakDtos.Add(BreakToDto(screenBreak, generalSettingsDto));
            }

            return breakDtos;
        }

        public static BreakDto BreakToDto(ScreenBreak screenBreak, GeneralSettingsDto generalSettings)
        {
            BreakSettingsDto settings = new BreakSettingsDto()
            {
                BreakInterval = screenBreak.Settings.BreakInterval,
                BreakLength = screenBreak.Settings.BreakLength,
                BreakQuotes = screenBreak.Settings.BreakQuotes,
                IsFullscreenBreak = screenBreak.Settings.IsFullScreenBreak,
                IsNotificationEnabled = generalSettings.IsNotificationEnabled,
                IsPermissiveNotification = generalSettings.IsPermissiveNotification,
                IsSkippable = screenBreak.Settings.IsSkippable,
                PostponeAmount = screenBreak.Settings.PostponeAmount,
                PostponeLength = screenBreak.Settings.PostponeLength,
                PreBreakNotificationLength = screenBreak.Settings.PreBreakNotificationLength,
                ScalingFactor = screenBreak.Settings.SmallBreakWindowScalingFactor
            };

            return new BreakDto()
            {
                Id = screenBreak.Id,
                Name = screenBreak.Name,
                Settings = settings
            };
        }
    }
}
