using BRCore.Settings;
using System;

namespace BRCore.MeasurementSystems
{
    [Serializable]
    internal class ScreenBreak
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public BreakSettings Settings { get; set; }
        public MeasurementType Type { get; set; }

        public ScreenBreak(int id, string name, BreakSettings settings, MeasurementType type)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            Type = type;
        }
    }
}
