using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;

namespace BlinkReminder.Windows.Controls
{
    public class MinuteTimeSpanUpDown : TimeSpanUpDown
    {
        protected override TimeSpan? ConvertTextToValue(string text)
        {
            if (string.IsNullOrEmpty(text))
                return null;

            int hours = 0;
            int minutes = Convert.ToInt32(text.Substring(0, 2));
            int seconds = Convert.ToInt32(text.Substring(3, 2));

            if (seconds >= 60)
            {
                minutes += seconds / 60;
                seconds = seconds % 60;
            }

            if (minutes >= 60)
            {
                hours = minutes / 60;
                minutes = minutes % 60;
            }

            return new TimeSpan(0, hours, minutes, seconds);
        }

        protected override string ConvertValueToText()
        {
            if (!this.Value.HasValue)
                return string.Empty;

            return this.Value.Value.ToString(@"mm\:ss");
        }

        protected override void OnIncrement()
        {
            if (this.Value.HasValue)
            {
                this.Value = this.Value.Value.Add(TimeSpan.FromMinutes(1));
            }
        }

        protected override void OnDecrement()
        {
            if (this.Value.HasValue)
            {
                this.Value = this.Value.Value.Add(TimeSpan.FromMinutes(-1));
            }
        }
        
        
    }
}
