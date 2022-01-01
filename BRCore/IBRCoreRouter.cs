using BRCore.Events;
using BRCore.Settings.DTO;
using BRCore.Update;
using System;

namespace BRCore
{
    public interface IBRCoreRouter
    {
        /// <summary>
        /// This event is fired when it's time to start a new break. 
        /// Everything needed to start the break is passed as an argument
        /// </summary>
        public event StartBreakEventHandle StartBreakEvent;

        /// <summary>
        /// Pauses measuring activity (therefore breaks) for the passed amount of time
        /// </summary>
        /// <param name="amount">The amount of time to pause measurements for</param>
        public void PauseMeasurement(TimeSpan pauseAmount);

        /// <summary>
        /// Returns an instance of the update runner which can update the application if there is a newever version
        /// </summary>
        /// <returns>The update runner</returns>
        public UpdateRunner GetUpdateRunner();

        /// <summary>
        /// Returns the current settings
        /// </summary>
        /// <returns>A dto of all the settings</returns>
        public SettingsDto GetSettings();

        /// <summary>
        /// Updates the current settings from the passed dto
        /// </summary>
        /// <param name="settings">The dto which holds all the data</param>
        public void UpdateSettings(SettingsDto settings);
    }
}
