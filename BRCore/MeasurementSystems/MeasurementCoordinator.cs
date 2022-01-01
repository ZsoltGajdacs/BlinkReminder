using BRCore.Settings.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace BRCore.MeasurementSystems
{
    internal class MeasurementCoordinator
    {
        public void InitMeasurement(SettingsDto settingsDto)
        {

        }

        public void PauseMeasurement(TimeSpan pauseAmount)
        {
            /*bool isPaused = true;

            // If the user chose timed pause
            if (pauseAmount.TotalSeconds > 0)
            {
                // Zero out the counter to know how long the pause should be going on
                TimerHandler.ResetCounterTime(ref pauseLengthSoFar);

                ActivateResumeBtn();
                SetTaskbarTooltip(TimeToPauseEnd() + TOOLTIP_PAUSE_MSG);
                PauseTimers(true);
            }
            // if this is going on till resume is pushed
            else if (pauseAmount.TotalMinutes.Equals(TimeSpan.FromMinutes(-1)))
            {
                ActivateResumeBtn();
                SetTaskbarTooltip(TOOLTIP_INDEF_PAUSE);
                PauseTimers(false);
            }
            // Or the user just canceled
            else
            {
                isPaused = false;
            }*/
        }
    }
}
