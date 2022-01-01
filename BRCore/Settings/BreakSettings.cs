using BRCore.MeasurementSystems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BRCore.Settings
{
    [Serializable]
    internal class BreakSettings
    {
        public TimeSpan BreakLength { get; set; }
        public TimeSpan BreakInterval { get; set; }

        public TimeSpan PostponeLength { get; set; }
        public int PostponeAmount { get; set; }

        public TimeSpan PreBreakNotificationLength { get; set; }

        public bool IsSkippable { get; set; }

        public bool IsFullScreenBreak { get; set; }
        public double SmallBreakWindowScalingFactor { get; set; }

        public List<Quote> BreakQuotes { get; set; }

        [JsonConstructor]
        private BreakSettings(TimeSpan breakLength, TimeSpan breakInterval, TimeSpan postponeLength,
                            int postponeAmount, TimeSpan preBreakNotificationLength, bool isSkippable,
                            bool isFullScreenBreak, double smallBreakWindowScalingFactor, List<Quote> breakQuotes)
        {
            BreakLength = breakLength;
            BreakInterval = breakInterval;
            PostponeLength = postponeLength;
            PostponeAmount = postponeAmount;
            PreBreakNotificationLength = preBreakNotificationLength;
            IsSkippable = isSkippable;
            IsFullScreenBreak = isFullScreenBreak;
            SmallBreakWindowScalingFactor = smallBreakWindowScalingFactor;
            BreakQuotes = breakQuotes ?? throw new ArgumentNullException(nameof(breakQuotes));
        }

        public BreakSettings()
        {
        }
    }
}
