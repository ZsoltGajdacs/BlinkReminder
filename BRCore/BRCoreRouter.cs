using BRCore.Events;
using BRCore.Mappers;
using BRCore.MeasurementSystems;
using BRCore.Settings;
using BRCore.Settings.DTO;
using BRCore.Update;
using System;

namespace BRCore
{
    public class BRCoreRouter : IBRCoreRouter
    {
        private readonly MeasurementCoordinator coordinator;

        public event StartBreakEventHandle StartBreakEvent;

        public BRCoreRouter()
        {
            coordinator = new MeasurementCoordinator();
            Init();
        }

        private void Init()
        {
            SettingsHolder holder = SettingsHolder.Instance;
            coordinator.InitMeasurement(SettingsMapper.SettingsToDto(holder.Settings));
        }

        public void PauseMeasurement(TimeSpan pauseAmount)
        {
            coordinator.PauseMeasurement(pauseAmount);
        }

        public UpdateRunner GetUpdateRunner()
        {
            return new UpdateRunner();
        }

        public SettingsDto GetSettings()
        {
            return SettingsMapper.SettingsToDto(SettingsHolder.Instance.Settings);
        }

        public void UpdateSettings(SettingsDto settings)
        {
            throw new NotImplementedException();
        }
    }
}
