using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BRCore.Settings
{
    [Serializable]
    public class Quote : INotifyPropertyChanged
    {
        private string _quoteText;
        private bool _isActive;
        public bool IsShort { get; set; }

        public Quote()
        {
            // Needed for serialization
        }

        public Quote(string quoteText, bool isActive, bool isShort)
        {
            _quoteText = quoteText;
            _isActive = isActive;
            IsShort = isShort;
        }

        #region Event handling
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Properties
        public string QuoteText
        {
            get
            {
                return _quoteText;
            }

            set
            {
                _quoteText = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsActive
        {
            get
            {
                return _isActive;
            }

            set
            {
                _isActive = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
    }
}
