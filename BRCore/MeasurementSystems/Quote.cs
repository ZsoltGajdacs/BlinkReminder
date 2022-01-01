using Newtonsoft.Json;
using System;
using ZsGUtils.UIHelpers;

namespace BRCore.MeasurementSystems
{
    [Serializable]
    public class Quote : BindableClass
    {
        private string _quoteText;
        private bool _isActive;

        public string QuoteText { get => _quoteText; set => SetAndNotifyPropertyChanged(ref _quoteText, value); }
        public bool IsActive { get => _isActive; set => SetAndNotifyPropertyChanged(ref _isActive, value); }

        public int BreakId { get; set; }

        [JsonConstructor]
        public Quote(string quoteText, bool isActive, int breakId)
        {
            _quoteText = quoteText;
            _isActive = isActive;
            BreakId = breakId;
        }
    }
}
