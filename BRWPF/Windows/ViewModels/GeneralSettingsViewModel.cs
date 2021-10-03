using ZsGUtils.UIHelpers;

namespace BRWPF.Windows.ViewModels
{
    public sealed class GeneralSettingsViewModel : BindableClass
    {
        private bool isTimerMode;
        private bool isActivityMode;
        private bool shouldBreakWhenFullScreenAppDetected;
        private bool indefPauseEnabled;
        private bool isNotificationEnabled;
        private bool isPermissiveNotification;
        private bool isFullscreenBreak;
        private int defaultPauseLength;
        private double scalingFactor;

        public bool IsTimerMode { get => isTimerMode; set => SetAndNotifyPropertyChanged(ref isTimerMode, value); }
        public bool IsActivityMode { get => isActivityMode; set => SetAndNotifyPropertyChanged(ref isActivityMode, value); }
        public bool ShouldBreakWhenFullScreenAppDetected { get => shouldBreakWhenFullScreenAppDetected; set => SetAndNotifyPropertyChanged(ref shouldBreakWhenFullScreenAppDetected, value); }
        public bool IndefPauseEnabled { get => indefPauseEnabled; set => SetAndNotifyPropertyChanged(ref indefPauseEnabled, value); }
        public bool IsNotificationEnabled { get => isNotificationEnabled; set => SetAndNotifyPropertyChanged(ref isNotificationEnabled, value); }
        public bool IsPermissiveNotification { get => isPermissiveNotification; set => SetAndNotifyPropertyChanged(ref isPermissiveNotification, value); }
        public bool IsFullscreenBreak { get => isFullscreenBreak; set => SetAndNotifyPropertyChanged(ref isFullscreenBreak, value); }

        public int DefaultPauseLength { get => defaultPauseLength; set => SetAndNotifyPropertyChanged(ref defaultPauseLength, value); }
        public double ScalingFactor { get => scalingFactor; set => SetAndNotifyPropertyChanged(ref scalingFactor, value); }
    }
}
