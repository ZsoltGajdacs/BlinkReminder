using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlinkReminder.Settings
{
    /// <summary>
    /// Default cycle times, interpreted as seconds
    /// </summary>
    enum CycleTimesEnum
    {
        ShortDisplayTime = 8, //8s
        ShortIntervalTime = 600, //10m
        LongDisplayTime = 500, //8m 20s
        LongIntervalTime = 3600, //60m
    }
}
