using BRCore.Settings.DTO;
using BRCore.Update;
using System;

namespace BRCore
{
    public class BRCoreRouter : IBRCoreRouter
    {
        private UpdateRunner updateRunner;

        #region Timer methods
        public void PauseTimers(TimeSpan pauseAmount)
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

        public void ResetTimers()
        {
            throw new NotImplementedException();
        }

        public void ResumeTimers()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Data transfer methods
        public UpdateRunner GetUpdateRunner()
        {
            return updateRunner ??= new UpdateRunner();
        }

        public GeneralSettingsDto GetSettings()
        {
            throw new NotImplementedException();
        }

        public void UpdateSettings(GeneralSettingsDto settings)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
