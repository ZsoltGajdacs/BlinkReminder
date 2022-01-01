using BRCore.MeasurementSystems;

namespace BRCore.Settings.DTO
{
    public class BreakDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BreakSettingsDto Settings { get; set; }
        public MeasurementType Type { get; set; }
    }
}
