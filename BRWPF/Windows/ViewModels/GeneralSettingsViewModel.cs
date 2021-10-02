using System.ComponentModel;

namespace BRWPF.Windows.ViewModels
{
    public class GeneralSettingsViewModel : INotifyPropertyChanged
    {
        public bool IsTimerMode { get; set; }
        public bool IsActivityMode { get; set; }
        public bool ShouldBreakWhenFullScreen { get; set; }
        public bool IndefPauseEnabled { get; set; }
        public bool IsNotificationEnabled { get; set; }
        public bool IsPermissiveNotification { get; set; }
        public bool IsFullscreenBreak { get; set; }

        public int DefaultPauseLength { get; set; }
        public double ScalingFactor { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
