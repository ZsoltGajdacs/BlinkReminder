using BRCore.MeasurementSystems;
using System;
using System.Collections.Generic;

namespace BRCore.Settings.DTO
{
    public sealed class SettingsDto
    {
        public GeneralSettingsDto GeneralSettingsDto { get; private set; }
        public List<BreakDto> BreakDtos { get; private set; }

        public SettingsDto(GeneralSettingsDto generalSettingsDto, List<BreakDto> breakDtos)
        {
            GeneralSettingsDto = generalSettingsDto ?? throw new ArgumentNullException(nameof(generalSettingsDto));
            BreakDtos = breakDtos ?? throw new ArgumentNullException(nameof(breakDtos));
        }
    }
}
