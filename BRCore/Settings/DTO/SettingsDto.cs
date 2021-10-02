using System;
using System.Collections.Generic;

namespace BRCore.Settings.DTO
{
    public sealed class SettingsDto
    {
        public GeneralSettingsDto GeneralSettingsDto { get; private set; }
        public List<BreakTimerSettingsDto> TimerSettingsDtos { get; private set; }

        public SettingsDto(GeneralSettingsDto generalSettingsDto, List<BreakTimerSettingsDto> timerSettingsDtos)
        {
            GeneralSettingsDto = generalSettingsDto ?? throw new ArgumentNullException(nameof(generalSettingsDto));
            TimerSettingsDtos = timerSettingsDtos ?? throw new ArgumentNullException(nameof(timerSettingsDtos));
        }
    }
}
