using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace BRCore.TimerBasedMeasurement
{
    public class TimerMeasurement : IMeasurementHandler
    {

        private Dictionary<string, Timer> timers;

        /// <summary>
        /// If the user changes time amounts in settings this method is called to set in logic
        /// </summary>
        private void DecideWhichClockToReset(string changedProperty)
        {
            switch (changedProperty)
            {
                case "ShortIntervalTime":
                    TimeSpan shortIntervalTimeSpan = settings.ShortIntervalTime;
                    SubstractForNotification(ref shortIntervalTimeSpan);
                    double shortInterval = shortIntervalTimeSpan.TotalMilliseconds;

                    if (shortInterval > 0)
                    {
                        TimerHandler.RestartTimer(ref shortIntervalTimer, shortInterval);
                    }
                    // If disabled
                    else
                    {
                        shortIntervalTimer.Stop();
                    }

                    TimerHandler.ResetCounterTime(ref shortBreakLengthSoFar);

                    break;

                case "LongIntervalTime":
                    TimeSpan longIntervalTimeSpan = settings.LongIntervalTime;
                    SubstractForNotification(ref longIntervalTimeSpan);

                    double longInterval = longIntervalTimeSpan.TotalMilliseconds;

                    if (longInterval > 0)
                    {
                        TimerHandler.RestartTimer(ref longIntervalTimer, longInterval);

                        EnableTaskbarOption(LongBreakStartItem);
                        SetTaskbarTooltip(Math.Round(settings.LongIntervalTime.TotalMinutes, 1) + TOOLTIP_LONG_MSG);
                    }
                    // If disabled
                    else
                    {
                        longIntervalTimer.Stop();
                        SetTaskbarTooltip(TOOLTIP_LONG_DISABLED);
                        DisableTaskbarOption(LongBreakStartItem);
                    }

                    TimerHandler.ResetCounterTime(ref longBreakLengthSoFar);

                    break;

                default:
                    break;
            }

            // Since there were changes restart the minute timer as well
            TimerHandler.RestartTimer(ref minuteTimer, HALF_MINUTE_IN_MS);
        }

        /// <summary>
        /// Pauses the short and long timers if timed pause.
        /// Short, long and minute timers if not timed pause.
        /// </summary>
        private void PauseTimers(bool isTimed)
        {
            if (isTimed)
            {
                List<Timer> timersToStop = new List<Timer>() { shortIntervalTimer, longIntervalTimer };
                TimerHandler.StopTimers(ref timersToStop);

                TimerHandler.RestartTimer(ref pauseTimer, pauseTotalLength.TotalMilliseconds);
            }
            else
            {
                List<Timer> timersToStop = new List<Timer>() { shortIntervalTimer, longIntervalTimer, minuteTimer };
                TimerHandler.StopTimers(ref timersToStop);
            }
        }

        /// <summary>
        /// Starts the long and short timers from where they were stopped. 
        /// Handles the case of pause state settings changes
        /// </summary>
        private void ResumeTimers()
        {
            // Come out of paused state
            isPaused = false;

            // Get the latest times from settings
            TimeSpan longIntervalTime = TimeSpan.FromMilliseconds(settings.LongIntervalTime.TotalMilliseconds);
            TimeSpan shortIntervalTime = TimeSpan.FromMilliseconds(settings.ShortIntervalTime.TotalMilliseconds);

            // Calculate remaining times
            TimeSpan longRemain = longIntervalTime - longBreakLengthSoFar;
            TimeSpan shortRemain = shortIntervalTime - shortBreakLengthSoFar;

            // Handle the case when the user changed the settings while in pause
            if (longRemain < TimeSpan.Zero)
            {
                longRemain = longIntervalTime;
            }

            if (shortRemain < TimeSpan.Zero)
            {
                shortRemain = shortIntervalTime;
            }

            // Account for possible pre-break notification
            SubstractForNotification(ref longRemain);
            SubstractForNotification(ref shortRemain);

            // Reset them
            TimerHandler.RestartTimer(ref shortIntervalTimer, shortRemain.TotalMilliseconds);
            TimerHandler.RestartTimer(ref longIntervalTimer, longRemain.TotalMilliseconds);
            TimerHandler.RestartTimer(ref minuteTimer, HALF_MINUTE_IN_MS);

            SetTaskbarTooltip(TimeToNextLongBreak() + TOOLTIP_LONG_MSG);
        }

        /// <summary>
        /// Resets the timer which generated the latest break event
        /// </summary>
        private void ResetElaspedTimer(ref TimeSpan longIntervalTime, ref TimeSpan shortIntervalTime)
        {
            SubstractForNotification(ref shortIntervalTime);

            if (isShortIntervalTimerDone)
            {
                TimerHandler.RestartTimer(ref shortIntervalTimer, shortIntervalTime.TotalMilliseconds);
                TimerHandler.ResetCounterTime(ref shortBreakLengthSoFar);

                isShortIntervalTimerDone = false;
            }

            if (isLongIntervalTimerDone)
            {
                SubstractForNotification(ref longIntervalTime);

                TimerHandler.RestartTimer(ref longIntervalTimer, longIntervalTime.TotalMilliseconds);
                TimerHandler.ResetCounterTime(ref longBreakLengthSoFar);

                // Restart the short timer too....
                if (settings.ShortIntervalTime > TimeSpan.Zero)
                {
                    TimerHandler.RestartTimer(ref shortIntervalTimer, shortIntervalTime.TotalMilliseconds);
                    TimerHandler.ResetCounterTime(ref shortBreakLengthSoFar);
                }

                isLongIntervalTimerDone = false;
            }

            // For "accuracy's" sake restart the minute timer too
            TimerHandler.RestartTimer(ref minuteTimer, HALF_MINUTE_IN_MS);

            // And refresh the toolbar tooltip
            SetTaskbarTooltip(TimeToNextLongBreak() + TOOLTIP_LONG_MSG);
        }

        /// <summary>
        /// Zeros out the counter whose timer is done
        /// </summary>
        private void ResetFinishedTimeCounter()
        {
            if (isShortIntervalTimerDone)
            {
                TimerHandler.ResetCounterTime(ref shortBreakLengthSoFar);
            }
            else if (isLongIntervalTimerDone)
            {
                TimerHandler.ResetCounterTime(ref longBreakLengthSoFar);
            }
        }

        /// <summary>
        /// Gives back the remaining minutes till the next long break
        /// </summary>
        private double TimeToNextLongBreak()
        {
            TimeSpan totalTime = settings.LongIntervalTime;
            TimeSpan remainingTime = totalTime - longBreakLengthSoFar;

            return Math.Round(remainingTime.TotalMinutes, 1);
        }

        /// <summary>
        /// Gives back the amount of minutes left until the end of the current pause state
        /// </summary>
        private double TimeToPauseEnd()
        {
            TimeSpan remainingTime = pauseTotalLength - pauseLengthSoFar;

            return Math.Round(remainingTime.TotalMinutes, 1);
        }

        /// <summary>
        /// If pre-break notification is enabled then the duration of the popup is substracted 
        /// from the time argument. 
        /// Usage: Call when giving value to the timers which govern the break times
        /// </summary>
        private void SubstractForNotification(ref TimeSpan time)
        {
            if (settings.IsNotificationEnabled && time > TimeSpan.Zero)
            {
                time -= settings.NotificationLength;
            }
        }

        /// <summary>
        /// Resets all the needed timers
        /// Usage: When the blocker window has closed, or when the break is postponed
        /// </summary>
        private void ResetTimers(TimeSpan longIntervalTime, TimeSpan shortIntervalTime)
        {
            // Order is important!
            ResetFinishedTimeCounter(); //Reset the related stopwatch too
            ResetElaspedTimer(ref longIntervalTime, ref shortIntervalTime); // Restart the clock that started the window that closed
            TimerHandler.RestartTimer(ref minuteTimer, HALF_MINUTE_IN_MS); // Reset the minute timer, so the counts are more precise
        }

        public void AddTimer(string name)
        {
            Timer newTimer = new Timer();
            timers.Add(name, );
        }

        public void RemoveTimer(string name)
        {
            throw new NotImplementedException();
        }
    }
}
