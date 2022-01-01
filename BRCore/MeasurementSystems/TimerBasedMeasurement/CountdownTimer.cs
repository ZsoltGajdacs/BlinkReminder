using System;
using System.Timers;
using ZsGUtils.UIHelpers;

namespace BRCore.MeasurementSystems.TimerBasedMeasurement
{
    /// <summary>
    /// Timer for the block window. Counts the remaining time left from the break
    /// </summary>
    public class CountdownTimer : BindableClass
    {
        private Timer timer; // Timer to display remaining time
        private TimeSpan duration; // Remaining time for display timer

        private string _timeToDisplay; // Variable for data binding

        /// <summary>
        /// Access to remaing to window close, preformatted hh:mm:ss
        /// </summary>
        public string TimeToDisplay { get => _timeToDisplay; set => SetAndNotifyPropertyChanged(ref _timeToDisplay, value); }

        public void Start(TimeSpan duration)
        {
            this.duration = duration;
            SetAndStartTimer();
            UpdateDurationAndDisplay();
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

        /// <summary>
        /// Called every second by the timer to update the display
        /// </summary>
        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            UpdateDurationAndDisplay();
        }

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
    }
}
