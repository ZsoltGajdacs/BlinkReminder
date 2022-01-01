namespace BRCore.Settings.DTO
{
    public sealed class GeneralSettingsDto
    {
        public bool ShouldBreakWhenFullScreenAppDetected { get; set; }

        public bool IndefPauseEnabled { get; set; }
        public int DefaultPauseLength { get; set; }

        public bool IsNotificationEnabled { get; set; }
        public bool IsPermissiveNotification { get; set; }
    }
}
