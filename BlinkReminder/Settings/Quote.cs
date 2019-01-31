using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BlinkReminder.Settings
{
    [Serializable]
    public class Quote : INotifyPropertyChanged
    {
        private int _index;
        private string _quoteText;
        private bool _isActive;

        public Quote()
        {
            // Needed for serialization
        }

        public Quote(string quoteText, bool isActive, int index)
        {
            _index = index;
            _quoteText = quoteText;
            _isActive = isActive;
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
        public int Index
        {
            get
            {
                return _index;
            }

            set
            {
                _index = value;
            }
        }

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
