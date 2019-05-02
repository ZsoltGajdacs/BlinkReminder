﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        /// <summary>
        /// Converts the second based times to easily readable format
        /// </summary>
        private void TimeFormatter(string propertyName)
        {
            switch (propertyName)
            {
                case ("ShortDisplayTime"):

                    if (ShortDisplayTime < TimeSpan.FromSeconds(10))
                    {
                        ShortDisplayTimeFormatted = ShortDisplayTime.ToString(TOSECONDSHORT);
                    }
                    else if (ShortDisplayTime < TimeSpan.FromSeconds(60))
                    {
                        ShortDisplayTimeFormatted = ShortDisplayTime.ToString(TOSECONDLONG);
                    }
                    else if (ShortDisplayTime < TimeSpan.FromMinutes(10))
                    {
                        ShortDisplayTimeFormatted = ShortDisplayTime.ToString(TOMINUTESHORT);
                    }
                    else
                    {
                        ShortDisplayTimeFormatted = ShortDisplayTime.ToString(TOMINUTELONG);
                    }

                    break;

                case ("ShortIntervalTime"):

                    if (ShortIntervalTime == TimeSpan.Zero)
                    {
                        ShortIntervalTimeFormatted = DISABLED_TEXT;
                    }
                    else if (ShortIntervalTime < TimeSpan.FromSeconds(60))
                    {
                        ShortIntervalTimeFormatted = ShortIntervalTime.ToString(TOSECONDLONG);
                    }
                    else if (ShortIntervalTime < TimeSpan.FromMinutes(10))
                    {
                        ShortIntervalTimeFormatted = ShortIntervalTime.ToString(TOMINUTESHORT);
                    }
                    else
                    {
                        ShortIntervalTimeFormatted = ShortIntervalTime.ToString(TOMINUTELONG);
                    }

                    break;

                case ("LongDisplayTime"):

                    if (LongDisplayTime < TimeSpan.FromSeconds(10))
                    {
                        LongDisplayTimeFormatted = LongDisplayTime.ToString(TOSECONDSHORT);
                    }
                    else if (LongDisplayTime < TimeSpan.FromSeconds(60))
                    {
                        LongDisplayTimeFormatted = LongDisplayTime.ToString(TOSECONDLONG);
                    }
                    else if (LongDisplayTime < TimeSpan.FromMinutes(10))
                    {
                        LongDisplayTimeFormatted = LongDisplayTime.ToString(TOMINUTESHORT);
                    }
                    else
                    {
                        LongDisplayTimeFormatted = LongDisplayTime.ToString(TOMINUTELONG);
                    }

                    break;

                case ("LongIntervalTime"):

                    if (LongIntervalTime == TimeSpan.Zero)
                    {
                        LongIntervalTimeFormatted = DISABLED_TEXT;
                    }
                    else if (LongIntervalTime < TimeSpan.FromMinutes(60))
                    {
                        LongIntervalTimeFormatted = LongIntervalTime.ToString(TOMINUTELONG);
                    }
                    else if (LongIntervalTime < TimeSpan.FromMinutes(10))
                    {
                        LongIntervalTimeFormatted = LongIntervalTime.ToString(TOMINUTESHORT);
                    }
                    else
                    {
                        LongIntervalTimeFormatted = LongIntervalTime.ToString(TOHOUR);
                    }

                    break;

                default:
                    break;
            }
        }
    }
}
