using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace BlinkReminder.Helpers
{
    internal static class TimerHandler
    {
        /// <summary>
        /// Restarts the given timer to the given time
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="milliSecondTime"></param>
        internal static void RestartTimer(ref Timer timer, double milliSecondTime)
        {
            timer.Stop();
            timer.Interval = milliSecondTime;
            timer.Start();
        }

        /// <summary>
        /// Stops the given timers
        /// </summary>
        internal static void StopTimers(ref List<Timer> timers)
        {
            foreach (Timer timer in timers)
            {
                timer.Stop();
            }
        }

        /// <summary>
        /// Increments the given counter by the given amount
        /// </summary>
        /// <param name="counter"></param>
        /// <param name="amount"></param>
        internal static void IncrementCounterTime(ref TimeSpan counter, double amount)
        {
            counter += TimeSpan.FromMilliseconds(amount);
        }

        /// <summary>
        /// Sets the given counter to Zero
        /// </summary>
        /// <param name="counter"></param>
        internal static void ResetCounterTime(ref TimeSpan counter)
        {
            counter = TimeSpan.Zero;
        }
    }
}
