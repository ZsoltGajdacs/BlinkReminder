using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlinkReminder.DTOs
{
    internal class SettingsDTO : INotifyPropertyChanged
    {
        // Logger
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // Text Consts
        private const string DISABLED_TEXT = "Timer disabled";

        // Consts for TimeSpan ToString
        private const string TOSECONDSHORT = @"s\s";
        private const string TOSECONDLONG = @"ss\s";
        private const string TOMINUTESHORT = @"m\m\:ss\s";
        private const string TOMINUTELONG = @"mm\m\:ss\s";
        private const string TOHOUR = @"h\h\:mm\m\:ss\s";

        // Second keepers for controls
        private long _shortDisplayAmount;
        private long _shortIntervalAmount;
        private long _longDisplayAmount;
        private long _longIntervalAmount;

        // Minute keepers for user guidance
        private string _shortDisplayTimeFormatted;
        private string _shortIntervalTimeFormatted;
        private string _LongDisplayTimeFormatted;
        private string _longIntervalTimeFormatted;

        // TimeSpan refs from settings so I can work with them
        private TimeSpan ShortDisplayTime { get; set; }
        private TimeSpan ShortIntervalTime { get; set; }
        private TimeSpan LongDisplayTime { get; set; }
        private TimeSpan LongIntervalTime { get; set; }

        // Time control min/max holders
        public long ShortDisplayMin { get; set; }
        public long ShortDisplayMax { get; set; }
        public long ShortIntervalMin { get; set; }
        public long ShortIntervalMax { get; set; }
        public long LongDisplayMin { get; set; }
        public long LongDisplayMax { get; set; }
        public long LongIntervalMin { get; set; }
        public long LongIntervalMax { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SettingsDTO(ref TimeSpan shortDisplayTime, ref TimeSpan shortIntervalTime, 
                            ref TimeSpan longDisplayTime, ref TimeSpan longIntervalTime)
        {
            PropertyChanged += SettingsDto_PropertyChanged;

            ShortDisplayTime = shortDisplayTime;
            ShortIntervalTime = shortIntervalTime;
            LongDisplayTime = longDisplayTime;
            LongIntervalTime = longIntervalTime;

            // Manually set long and short interval helper texts if they are set to zero
            if (ShortIntervalTime == TimeSpan.Zero) ShortIntervalTimeFormatted = DISABLED_TEXT;
            if (LongIntervalTime == TimeSpan.Zero) LongIntervalTimeFormatted = DISABLED_TEXT;

            ShortDisplayAmount = (long)ShortDisplayTime.TotalSeconds;
            ShortIntervalAmount = (long)ShortIntervalTime.TotalSeconds;
            LongDisplayAmount = (long)LongDisplayTime.TotalSeconds;
            LongIntervalAmount = (long)LongIntervalTime.TotalSeconds;

            // Set min/max values to default safe
            ShortDisplayMin = 0;
            ShortIntervalMin = 0;
            LongDisplayMin = 0;
            LongIntervalMin = 0;

            ShortDisplayMax = 10000;
            ShortIntervalMax = 10000;
            LongDisplayMax = 10000;
            LongIntervalMax = 10000;
        }

        #region Property changed handler

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Event handler

        private void SettingsDto_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            TimeFormatter(e.PropertyName);
            LimitSetter(e.PropertyName);
        }

        #endregion

        #region Minute display helpers

        public string ShortDisplayTimeFormatted
        {
            get
            {
                return _shortDisplayTimeFormatted;
            }
            set
            {
                _shortDisplayTimeFormatted = value;
                NotifyPropertyChanged();
            }
        }

        public string ShortIntervalTimeFormatted
        {
            get
            {
                return _shortIntervalTimeFormatted;
            }
            set
            {
                _shortIntervalTimeFormatted = value;
                NotifyPropertyChanged();
            }
        }

        public string LongDisplayTimeFormatted
        {
            get
            {
                return _LongDisplayTimeFormatted;
            }
            set
            {
                _LongDisplayTimeFormatted = value;
                NotifyPropertyChanged();
            }
        }

        public string LongIntervalTimeFormatted
        {
            get
            {
                return _longIntervalTimeFormatted;
            }
            set
            {
                _longIntervalTimeFormatted = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Control Properties

        public long ShortDisplayAmount
        {
            get
            {
                return _shortDisplayAmount;
            }

            set
            {
                _shortDisplayAmount = value;
                NotifyPropertyChanged();
            }
        }

        public long ShortIntervalAmount
        {
            get
            {
                return _shortIntervalAmount;
            }

            set
            {
                _shortIntervalAmount = value;
                NotifyPropertyChanged();
            }
        }

        public long LongDisplayAmount
        {
            get
            {
                return _longDisplayAmount;
            }

            set
            {
                _longDisplayAmount = value;
                NotifyPropertyChanged();
            }
        }

        public long LongIntervalAmount
        {
            get
            {
                return _longIntervalAmount;
            }

            set
            {
                _longIntervalAmount = value;
                NotifyPropertyChanged();
            }
        }

        #endregion

        #region Helpers
        /// <summary>
        /// Converts the second based times to easily readable format
        /// </summary>
        private void TimeFormatter(string propertyName)
        {
            TimeSpan time;
            switch (propertyName)
            {
                case ("ShortDisplayAmount"):
                    
                    time = TimeSpan.FromSeconds(ShortDisplayAmount);
                    if (time < TimeSpan.FromSeconds(10))
                    {
                        ShortDisplayTimeFormatted = time.ToString(TOSECONDSHORT);
                    }
                    else if (ShortDisplayTime < TimeSpan.FromSeconds(60))
                    {
                        ShortDisplayTimeFormatted = time.ToString(TOSECONDLONG);
                    }
                    else if (ShortDisplayTime < TimeSpan.FromMinutes(10))
                    {
                        ShortDisplayTimeFormatted = time.ToString(TOMINUTESHORT);
                    }
                    else
                    {
                        ShortDisplayTimeFormatted = time.ToString(TOMINUTELONG);
                    }

                    break;

                case ("ShortIntervalAmount"):

                    time = TimeSpan.FromSeconds(ShortIntervalAmount);
                    if (time == TimeSpan.Zero)
                    {
                        ShortIntervalTimeFormatted = DISABLED_TEXT;
                    }
                    else if (time < TimeSpan.FromSeconds(60))
                    {
                        ShortIntervalTimeFormatted = time.ToString(TOSECONDLONG);
                    }
                    else if (time < TimeSpan.FromMinutes(10))
                    {
                        ShortIntervalTimeFormatted = time.ToString(TOMINUTESHORT);
                    }
                    else
                    {
                        ShortIntervalTimeFormatted = time.ToString(TOMINUTELONG);
                    }

                    break;

                case ("LongDisplayAmount"):

                    time = TimeSpan.FromSeconds(LongDisplayAmount);
                    if (time < TimeSpan.FromSeconds(10))
                    {
                        LongDisplayTimeFormatted = time.ToString(TOSECONDSHORT);
                    }
                    else if (time < TimeSpan.FromSeconds(60))
                    {
                        LongDisplayTimeFormatted = time.ToString(TOSECONDLONG);
                    }
                    else if (time < TimeSpan.FromMinutes(10))
                    {
                        LongDisplayTimeFormatted = time.ToString(TOMINUTESHORT);
                    }
                    else
                    {
                        LongDisplayTimeFormatted = time.ToString(TOMINUTELONG);
                    }

                    break;

                case ("LongIntervalAmount"):

                    time = TimeSpan.FromSeconds(LongIntervalAmount);
                    if (time == TimeSpan.Zero)
                    {
                        LongIntervalTimeFormatted = DISABLED_TEXT;
                    }
                    else if (time < TimeSpan.FromMinutes(60))
                    {
                        LongIntervalTimeFormatted = time.ToString(TOMINUTELONG);
                    }
                    else if (time < TimeSpan.FromMinutes(10))
                    {
                        LongIntervalTimeFormatted = time.ToString(TOMINUTESHORT);
                    }
                    else
                    {
                        LongIntervalTimeFormatted = time.ToString(TOHOUR);
                    }

                    break;

                default:
                    break;
            }
        }

        /// <summary>
        /// Sets the min/max amounts of break intervals so short and long ones can't flip
        /// </summary>
        private void LimitSetter(string propertyName)
        {
            switch (propertyName)
            {
                case ("ShortDisplayAmount"):
                    // TODO: Figure out the usefulness of this
                    break;

                case ("ShortIntervalAmount"):
                    LongIntervalMin = ShortIntervalAmount + 1;
                    break;

                case ("LongDisplayAmount"):
                    // TODO: Figure out the usefulness of this
                    break;

                case ("LongIntervalAmount"):
                    ShortIntervalMax = LongIntervalAmount - 1;
                    break;

                default:
                    break;
            }
        }

        #endregion
    }
}
