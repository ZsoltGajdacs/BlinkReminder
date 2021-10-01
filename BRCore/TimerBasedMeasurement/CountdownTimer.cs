using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Timers;

namespace BRCore.TimerBasedMeasurement
{
    /// <summary>
    /// Timer for the block window. Counts the remaining time left from the break
    /// </summary>
    class CountdownTimer : INotifyPropertyChanged
    {
        private Timer timer; // Timer to display remaining time
        private TimeSpan duration; // Remaining time for display timer

        private string _timeToDisplay; // Variable for data binding

        public event PropertyChangedEventHandler PropertyChanged;

        public CountdownTimer(TimeSpan duration)
        {
            this.duration = duration;
            UpdateDurationAndDisplay();
            SetAndStartTimer();
        }

        #region Property changed handler

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Accessors

        /// <summary>
        /// Access to remaing to window close, preformatted hh:mm:ss
        /// </summary>
        public string TimeToDisplay
        {
            get
            {
                return _timeToDisplay;
            }

            set
            {
                if (!value.Equals(_timeToDisplay))
                {
                    _timeToDisplay = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Startup methods

        /// <summary>
        /// Sets the TimeToDisplay property. 
        /// Stops the clock if zero reached
        /// </summary>
        private void UpdateDurationAndDisplay()
        {
            if (duration >= TimeSpan.FromHours(1))
            {
                TimeToDisplay = duration.ToString(@"h\:mm\:ss");
            }
            else if (duration >= TimeSpan.FromMinutes(10))
            {
                TimeToDisplay = duration.ToString(@"mm\:ss");
            }
            else if (duration >= TimeSpan.FromMinutes(1))
            {
                TimeToDisplay = duration.ToString(@"m\:ss");
            }
            else
            {
                TimeToDisplay = duration.ToString("%s");
            }

            if (duration == TimeSpan.Zero)
            {
                timer.Stop();
            }

            duration = duration.Add(TimeSpan.FromSeconds(-1));
        }

        /// <summary>
        /// Sets up a dispatcherTimer and starts it
        /// </summary>
        private void SetAndStartTimer()
        {
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = TimeSpan.FromSeconds(1).TotalMilliseconds;
            timer.AutoReset = true;

            timer.Start();
        }
        #endregion

        #region Event methods
        /// <summary>
        /// Called every second by the timer to update the display
        /// </summary>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateDurationAndDisplay();
        }
        #endregion
    }
}
