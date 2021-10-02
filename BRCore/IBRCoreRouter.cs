using BRCore.Settings;
using BRCore.Settings.DTO;
using BRCore.Update;
using System;
using System.Collections.Generic;
using System.Text;

namespace BRCore
{
    public interface IBRCoreRouter
    {
        public void PauseTimers(TimeSpan amount);
        public void ResumeTimers();
        public void ResetTimers();

        public UpdateRunner GetUpdateRunner();

        public SettingsDto GetSettings();
        public void RefreshSettings(SettingsDto settings);
    }
}
