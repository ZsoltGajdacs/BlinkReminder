using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BlinkReminder.DTOs
{
    /// <summary>
    /// Intermediary between settings and the UI. TODO: Come up with a better name, 
    /// as this is not really a DTO!
    /// </summary>
    internal class SettingsDTO : INotifyPropertyChanged, IDisposable
    {
        // Logger
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // Text Consts
        private const string DISABLED_TEXT = "Timer disabled";

        // Numeric Consts
        private const double MILLISECONDS_TO_SAVE = 3000;

        // Consts for TimeSpan ToString
        private const string TOSECONDSHORT = @"s\s";
        private const string TOSECONDLONG = @"ss\s";
        private const string TOMINUTESHORT = @"m\m\:ss\s";
        private const string TOMINUTELONG = @"mm\m\:ss\s";
        private const string TOHOUR = @"h\h\:mm\m\:ss\s";

        // Second keepers for controls
        private long _shortDisplayAmount;
        private double _shortIntervalAmount;
        private double _longDisplayAmount;
        private double _longIntervalAmount;

        // Secondary settings controls
        private int _lockLengthTimeExtent;
        private int _postponeLength;
        private int _postponeAmount;
        private int _notificationLength;


        // Time control min/max holders
        public long ShortDisplayMin { get; set; }
        public long ShortDisplayMax { get; set; }
        public long ShortIntervalMin { get; set; }
        public long ShortIntervalMax { get; set; }
        public long LongDisplayMin { get; set; }
        public long LongDisplayMax { get; set; }
        public long LongIntervalMin { get; set; }
        public long LongIntervalMax { get; set; }

        // Timer to know when to update the main time variables
        public Timer UserInactivityTimer { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        #region CTOR
        public SettingsDTO(ref TimeSpan shortDisplayTime, ref TimeSpan shortIntervalTime, 
                            ref TimeSpan longDisplayTime, ref TimeSpan longIntervalTime,
                            ref TimeSpan lockLengthTimeExtent, ref TimeSpan postponeLength, 
                            ref int postponeAmount, ref TimeSpan notificationLength)
        {
            PropertyChanged += SettingsDto_PropertyChanged;

            // Set up inactivity timer, so I know when it's time to write out
            // values to settings
            UserInactivityTimer = new Timer(MILLISECONDS_TO_SAVE);
            UserInactivityTimer.AutoReset = false;

            // Manually set long and short interval helper texts if they are set to zero
            // TODO: Figure out how to show that timers are disabled
            //if (shortIntervalTime == TimeSpan.Zero) ShortIntervalTimeFormatted = DISABLED_TEXT;
            //if (longIntervalTime == TimeSpan.Zero) LongIntervalTimeFormatted = DISABLED_TEXT;

            // Set internal keepers from settings
            ShortDisplayAmount = (long)shortDisplayTime.TotalSeconds;
            ShortIntervalAmount = shortIntervalTime.TotalMinutes;
            LongDisplayAmount = longDisplayTime.TotalMinutes;
            LongIntervalAmount = longIntervalTime.TotalMinutes;

            // Set supplementary times
            LockLengthTimeExtent = (int)lockLengthTimeExtent.TotalMinutes;
            PostponeLength = (int)postponeLength.TotalMinutes;
            PostponeAmount = postponeAmount;
            NotificationLength = (int)notificationLength.TotalSeconds;

            // Set min/max values to default safe
            // NOT USED RIGHT NOW
            ShortDisplayMin = 0;
            ShortIntervalMin = 0;
            LongDisplayMin = 0;
            LongIntervalMin = 0;

            ShortDisplayMax = 10000;
            ShortIntervalMax = 10000;
            LongDisplayMax = 10000;
            LongIntervalMax = 10000;
        }
        #endregion

        #region Property changed handler

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Event handler

        private void SettingsDto_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UserInactivityTimer.Stop();

            LimitSetter(e.PropertyName);
            
            UserInactivityTimer.Start();
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

        public double ShortIntervalAmount
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

        public double LongDisplayAmount
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

        public double LongIntervalAmount
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

        /// <summary>
        /// The amount of minutes below which a locked screen is considered a short break 
        /// or above it a long one
        /// </summary>
        public int LockLengthTimeExtent
        {
            get
            {
                return _lockLengthTimeExtent;
            }

            set
            {
                if (value != _lockLengthTimeExtent)
                {
                    _lockLengthTimeExtent = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The amount of minutes a break will be postponed by
        /// </summary>
        public int PostponeLength
        {
            get
            {
                return _postponeLength;
            }

            set
            {
                if (value != _postponeLength)
                {
                    _postponeLength = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The amount of times a break can be postponed before it's cancelled entirely
        /// </summary>
        public int PostponeAmount
        {
            get
            {
                return _postponeAmount;
            }

            set
            {
                if (value != _postponeAmount)
                {
                    _postponeAmount = value;
                    NotifyPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Number of seconds a pre-break notification lasts
        /// </summary>
        public int NotificationLength
        {
            get
            {
                return _notificationLength;
            }

            set
            {
                if (value != _notificationLength)
                {
                    _notificationLength = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Helpers

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
                    LongIntervalMin = (long)Math.Round(ShortIntervalAmount + 1, 1);
                    break;

                case ("LongDisplayAmount"):
                    // TODO: Figure out the usefulness of this
                    break;

                case ("LongIntervalAmount"):
                    ShortIntervalMax = (long)Math.Round(LongIntervalAmount - 1, 1);
                    break;

                default:
                    break;
            }
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    UserInactivityTimer.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SettingsDTO() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
