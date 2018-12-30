using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlinkReminder.Settings
{
    /// <summary>
    /// Singleton pattern
    /// If no settings from user defaults are:
    /// Short Display: 8s
    /// Short Interval: 10m
    /// Long Display: 8m 20s
    /// Long Interval: 60m
    /// </summary>
    class UserSettings
    {
        // Times are interpreted as milliseconds
        internal long ShortDisplayTime { get; set; }
        internal long ShortIntervalTime { get; set; }
        internal long LongDisplayTime { get; set; }
        internal long LongIntervalTime { get; set; }

        internal bool IsShortSkippable { get; set; }
        internal bool IsLongSkippable { get; set; }

        private List<string> shortBreakQuotes;
        private List<string> longBreakQuotes;

        #region Singleton stuff
        private static readonly Lazy<UserSettings> lazy = new Lazy<UserSettings>(() => new UserSettings());

        public static UserSettings Instance { get { return lazy.Value; } }
        #endregion

        private UserSettings()
        {
            ShortDisplayTime = (long)CycleTimesEnum.ShortDisplayTime;
            ShortIntervalTime = (long)CycleTimesEnum.ShortIntervalTime;
            LongDisplayTime = (long)CycleTimesEnum.LongDisplayTime;
            LongIntervalTime = (long)CycleTimesEnum.LongIntervalTime;

            IsShortSkippable = false;
            IsLongSkippable = true;

            shortBreakQuotes = new List<string>();
            longBreakQuotes = new List<string>();
        }

        internal string GetShortQuote()
        {
            // Temporary stuff
            return "This is a short break";
        }

        internal string GetLongQuote()
        {
            // Temp
            return "This is a long break";
        }

    }
}
