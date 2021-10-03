using AutoMapper;
using BRCore.Settings.DTO;
using BRWPF.Windows.ViewModels;
using System;

namespace BRWPF.Mappers
{
    public class SettingsMapper
    {
        private readonly MapperConfiguration settingsDtoToGeneralSettingsVm;
        private readonly MapperConfiguration generalSettingsVmTosettingsDto;
        private IMapper settingsDtoToGeneralSettingsVmMapper;
        private IMapper generalSettingsVmTosettingsDtoMapper;

        private readonly MapperConfiguration breakTimerSettingsDtoToBreakTimerVm;
        private readonly MapperConfiguration breakTimerVMToBreakTimerSettingsDto;
        private IMapper breakTimerSettingsDtoToBreakTimerVmMapper;
        private IMapper breakTimerVMToBreakTimerSettingsDtoMapper;


        #region Singleton stuff
        public static SettingsMapper Instance { get { return lazy.Value; } }

        private static readonly Lazy<SettingsMapper> lazy = new Lazy<SettingsMapper>(() =>
        {
            var settingsDtoToGeneralSettingsVm = new MapperConfiguration(cfg => cfg.CreateMap<GeneralSettingsDto, GeneralSettingsViewModel>());
            var generalSettingsVmTosettingsDto = new MapperConfiguration(cfg => cfg.CreateMap<GeneralSettingsViewModel, GeneralSettingsDto>());

            var breakTimerSettingsDtoToBreakTimerVm = new MapperConfiguration(cfg => cfg.CreateMap<BreakTimerSettingsDto, BreakSettingsViewModel>());
            var breakTimerVMToBreakTimerSettingsDto = new MapperConfiguration(cfg => cfg.CreateMap<BreakSettingsViewModel, BreakTimerSettingsDto>());

            return new SettingsMapper(settingsDtoToGeneralSettingsVm, generalSettingsVmTosettingsDto,
                breakTimerSettingsDtoToBreakTimerVm, breakTimerVMToBreakTimerSettingsDto);
        });

        private SettingsMapper(MapperConfiguration settingsDtoToGeneralSettingsVm,
            MapperConfiguration generalSettingsVmTosettingsDto,
            MapperConfiguration breakTimerSettingsDtoToBreakTimerVm,
            MapperConfiguration breakTimerVMToBreakTimerSettingsDto)
        {
            this.settingsDtoToGeneralSettingsVm = settingsDtoToGeneralSettingsVm;
            this.generalSettingsVmTosettingsDto = generalSettingsVmTosettingsDto;
            this.breakTimerSettingsDtoToBreakTimerVm = breakTimerSettingsDtoToBreakTimerVm;
            this.breakTimerVMToBreakTimerSettingsDto = breakTimerVMToBreakTimerSettingsDto;
        }
        #endregion

        public GeneralSettingsViewModel ToGeneralSettingsViewModel(GeneralSettingsDto settingsDto)
        {
            if (settingsDtoToGeneralSettingsVmMapper == null)
            {
                settingsDtoToGeneralSettingsVmMapper = settingsDtoToGeneralSettingsVm.CreateMapper();
            }

            return settingsDtoToGeneralSettingsVmMapper.Map<GeneralSettingsViewModel>(settingsDto);
        }

        public GeneralSettingsDto ToGeneralSettingsDto(GeneralSettingsViewModel generalSettingsViewModel)
        {
            if (generalSettingsVmTosettingsDtoMapper == null)
            {
                generalSettingsVmTosettingsDtoMapper = generalSettingsVmTosettingsDto.CreateMapper();
            }

            return generalSettingsVmTosettingsDtoMapper.Map<GeneralSettingsDto>(generalSettingsViewModel);
        }

        public BreakSettingsViewModel ToBreakTimerVM(BreakTimerSettingsDto breakTimerDto)
        {
            if (breakTimerSettingsDtoToBreakTimerVmMapper == null)
            {
                breakTimerSettingsDtoToBreakTimerVmMapper = breakTimerSettingsDtoToBreakTimerVm.CreateMapper();
            }

            return breakTimerSettingsDtoToBreakTimerVmMapper.Map<BreakSettingsViewModel>(breakTimerDto);
        }

        public BreakTimerSettingsDto ToBreakSettingsDto(BreakSettingsViewModel breakSettingsViewModel)
        {
            if (breakTimerVMToBreakTimerSettingsDtoMapper == null)
            {
                breakTimerVMToBreakTimerSettingsDtoMapper = breakTimerVMToBreakTimerSettingsDto.CreateMapper();
            }

            return breakTimerVMToBreakTimerSettingsDtoMapper.Map<BreakTimerSettingsDto>(breakSettingsViewModel);
        }
    }
}
