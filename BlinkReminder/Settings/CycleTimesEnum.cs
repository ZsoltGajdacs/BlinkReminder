using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlinkReminder.Settings
{
    /// <summary>
    /// Default cycle times, interpreted as milliseconds
    /// </summary>
    enum CycleTimesEnum
    {
        ShortDisplayTime = 8000, //8s
        ShortIntervalTime = 600000, //10m
        LongDisplayTime = 500000, //8m 20s
        LongIntervalTime = 3600000, //60m
    }
}
