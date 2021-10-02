using System;
using System.Collections.Generic;

namespace BRCore.Settings.DTO
{
    public sealed class BreakTimerSettingsDto
    {
        public TimeSpan BreakLength { get; set; }
        public TimeSpan BreakInterval { get; set; }

        public TimeSpan PostponeLength { get; set; }
        public int PostponeAmount { get; set; }

        public TimeSpan PreBreakNotificationLength { get; set; }

        public bool IsSkippable { get; set; }

        public bool IsFullScreenBreak { get; set; }
        public double SmallBreakWindowScalingFactor { get; set; }

        public bool IsPermissiveNotification { get; set; }

        public List<Quote> BreakQuotes { get; set; }
    }
}
