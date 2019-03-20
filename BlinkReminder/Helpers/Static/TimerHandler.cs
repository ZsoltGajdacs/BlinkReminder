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
        /// Resets the given timer to the given time
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="time"></param>
        internal static void ResetTimer(ref Timer timer, long time)
        {
            timer.Stop();
            timer.Interval = time;
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
    }
}
