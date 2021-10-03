using System;
using ZsGUtils.UIHelpers;

namespace BRCore.Settings
{
    [Serializable]
    public class Quote : BindableClass
    {
        private string _quoteText;
        private bool _isActive;
        public int BreakId { get; set; }

        public Quote() { /* Needed for serialization */ }

        public Quote(string quoteText, bool isActive, int breakId)
        {
            _quoteText = quoteText;
            _isActive = isActive;
            BreakId = breakId;
        }

        #region Properties
        public string QuoteText
        {
            get => _quoteText;
            set => SetAndNotifyPropertyChanged(ref _quoteText, value);
        }

        public bool IsActive
        {
            get => _isActive;
            set => SetAndNotifyPropertyChanged(ref _isActive, value);
        }
        #endregion
    }
}
