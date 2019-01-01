using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlinkReminder.Settings
{
    public class UserSettings : INotifyPropertyChanged
    {
        // Times are interpreted as seconds
        private long shortDisplayTime;
        private long shortIntervalTime;
        private long longDisplayTime;
        private long longIntervalTime;

        private bool isShortSkippable;
        private bool isLongSkippable;

        private List<string> shortBreakQuotes;
        private List<string> longBreakQuotes;

        public event PropertyChangedEventHandler PropertyChanged;

        #region Singleton stuff
        private static readonly Lazy<UserSettings> lazy = new Lazy<UserSettings>(() => new UserSettings());

        public static UserSettings Instance { get { return lazy.Value; } }
        #endregion

        #region Property changed handler

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Accessor properties

        /// <summary>
        /// Gives back Seconds
        /// </summary>
        public long ShortDisplayTime
        {
            get
            {
                return this.shortDisplayTime;
            }

            set
            {
                if (value != this.shortDisplayTime)
                {
                    this.shortDisplayTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gives back Seconds
        /// </summary>
        public long ShortIntervalTime
        {
            get
            {
                return this.shortIntervalTime;
            }

            set
            {
                if (value != this.shortIntervalTime)
                {
                    this.shortIntervalTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gives back Seconds
        /// </summary>
        public long LongDisplayTime
        {
            get
            {
                return this.longDisplayTime;
            }

            set
            {
                if (value != this.longDisplayTime)
                {
                    this.longDisplayTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gives back Seconds
        /// </summary>
        public long LongIntervalTime
        {
            get
            {
                return this.longIntervalTime;
            }

            set
            {
                if (value != this.longIntervalTime)
                {
                    this.longIntervalTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsShortSkippable
        {
            get
            {
                return this.isShortSkippable;
            }

            set
            {
                if (value != this.isShortSkippable)
                {
                    this.isShortSkippable = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsLongSkippable
        {
            get
            {
                return this.isLongSkippable;
            }

            set
            {
                if (value != isLongSkippable)
                {
                    this.isLongSkippable = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Milliseconds getters
        public long GetShortDisplayMillisecond()
        {
            return shortDisplayTime * 1000;
        }

        public long GetLongDisplayMillisecond()
        {
            return longDisplayTime * 1000;
        }

        public long GetShortIntervalMillisecond()
        {
            return shortIntervalTime * 1000;
        }

        public long GetLongIntervalMillisecond()
        {
            return longIntervalTime * 1000;
        }
        #endregion

        private UserSettings()
        {
            SetDefaults();
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

        private void SetDefaults()
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

    }
}
