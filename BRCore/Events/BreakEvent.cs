using BRCore.MeasurementSystems.TimerBasedMeasurement;
using BRCore.Settings.DTO;
using System;

namespace BRCore.Events
{
    public delegate void StartBreakEventHandle(BreakEventArgs eventArgs);

    public class BreakEventArgs : EventArgs
    {
        private readonly BreakDto _breakData;
        private readonly CountdownTimer _displayTimer;

        public BreakDto BreakData { get => _breakData; }
        public CountdownTimer DisplayTimer { get => _displayTimer; }

        public BreakEventArgs(BreakDto breakData)
        {
            this._breakData = breakData ?? throw new ArgumentNullException(nameof(breakData));
            this._displayTimer = new CountdownTimer();
        }
    }
}
