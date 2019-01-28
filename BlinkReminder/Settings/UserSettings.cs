using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace BlinkReminder.Settings
{
    /// <summary>
    /// Keeps the current settings
    /// </summary>
    [Serializable]
    public class UserSettings : INotifyPropertyChanged, ISerializable
    {
        // Consts for TimeSpan ToString
        private const string TOSECONDSHORT = @"s\s";
        private const string TOSECONDLONG = @"ss\s";
        private const string TOMINUTESHORT = @"m\m\:ss\s";
        private const string TOMINUTELONG = @"mm\m\:ss\s";
        private const string TOHOUR = @"h\h\:mm\m\:ss\s";

        // Const for serialization
        internal const string SETTINGSFILEPATH = "Settings.brs";

        // Times are interpreted as seconds
        private long _shortDisplayTime;
        private long _shortIntervalTime;
        private long _longDisplayTime;
        private long _longIntervalTime;

        // Minute keepers for user guidance
        private string _shortDisplayTimeFormatted;
        private string _shortIntervalTimeFormatted;
        private string _LongDisplayTimeFormatted;
        private string _longIntervalTimeFormatted;

        // For setting whether the breaks are skippable
        private bool _isShortSkippable;
        private bool _isLongSkippable;

        // For keeping the quotes that appear during breaks
        private List<string> shortBreakQuotes;
        private List<string> longBreakQuotes;

        // For checking if breaks should occur when there is a fullscreen app running
        private bool _shouldBrakeWhenFullScreen;

        private Random rand;

        // Event handler for MVVM support
        public event PropertyChangedEventHandler PropertyChanged;

        #region Singleton stuff
        private static readonly Lazy<UserSettings> lazy = new Lazy<UserSettings>(() => 
        {
            if (File.Exists(SETTINGSFILEPATH) && new FileInfo(SETTINGSFILEPATH).Length > 0)
            {
                IFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(SETTINGSFILEPATH, FileMode.Open, FileAccess.Read);

                return (UserSettings)formatter.Deserialize(stream);
            }
            else
            {
                return new UserSettings();
            }
            
            
        });

        public static UserSettings Instance { get { return lazy.Value; } }
        #endregion

        #region Constructors
        private UserSettings()
        {
            PropertyChanged += UserSettings_PropertyChanged;

            ShortDisplayTime = (long)CycleTimesEnum.ShortDisplayTime;
            ShortIntervalTime = (long)CycleTimesEnum.ShortIntervalTime;
            LongDisplayTime = (long)CycleTimesEnum.LongDisplayTime;
            LongIntervalTime = (long)CycleTimesEnum.LongIntervalTime;

            IsShortSkippable = false;
            IsLongSkippable = true;
            ShouldBreakWhenFullScreen = true;

            shortBreakQuotes = new List<string>() { "This is a short break" };
            longBreakQuotes = new List<string>() { "This is a long break" };

            rand = new Random();
        }

        private UserSettings(SerializationInfo info, StreamingContext context)
        {
            PropertyChanged += UserSettings_PropertyChanged;

            ShortDisplayTime = (long)info.GetValue("sdt", typeof(long));
            ShortIntervalTime = (long)info.GetValue("sit", typeof(long));
            LongDisplayTime = (long)info.GetValue("ldt", typeof(long));
            LongIntervalTime = (long)info.GetValue("lit", typeof(long));

            IsShortSkippable = (bool)info.GetValue("iss", typeof(bool));
            IsLongSkippable = (bool)info.GetValue("ils", typeof(bool));
            ShouldBreakWhenFullScreen = (bool)info.GetValue("sbwfs", typeof(bool));

            shortBreakQuotes = ((string[])info.GetValue("sbq", typeof(string[]))).ToList();
            longBreakQuotes = ((string[])info.GetValue("lbq", typeof(string[]))).ToList();

            rand = new Random();
        }
        #endregion

        #region Property changed handler

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Event handler

        private void UserSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            TimeConverter(e.PropertyName);
        }

        #endregion

        #region Accessor properties

        #region Time setters

        /// <summary>
        /// Gives back Seconds
        /// </summary>
        public long ShortDisplayTime
        {
            get
            {
                return this._shortDisplayTime;
            }

            set
            {
                if (value != this._shortDisplayTime)
                {
                    this._shortDisplayTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gives back Seconds
        /// </summary>
        public long ShortIntervalTime
        {
            get
            {
                return this._shortIntervalTime;
            }

            set
            {
                if (value != this._shortIntervalTime)
                {
                    this._shortIntervalTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gives back Seconds
        /// </summary>
        public long LongDisplayTime
        {
            get
            {
                return this._longDisplayTime;
            }

            set
            {
                if (value != this._longDisplayTime)
                {
                    this._longDisplayTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        
        /// <summary>
        /// Gives back Seconds
        /// </summary>
        public long LongIntervalTime
        {
            get
            {
                return this._longIntervalTime;
            }

            set
            {
                if (value != this._longIntervalTime)
                {
                    this._longIntervalTime = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        public bool IsShortSkippable
        {
            get
            {
                return this._isShortSkippable;
            }

            set
            {
                if (value != this._isShortSkippable)
                {
                    this._isShortSkippable = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsLongSkippable
        {
            get
            {
                return this._isLongSkippable;
            }

            set
            {
                if (value != _isLongSkippable)
                {
                    this._isLongSkippable = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool ShouldBreakWhenFullScreen
        {
            get
            {
                return _shouldBrakeWhenFullScreen;
            }
            set
            {
                _shouldBrakeWhenFullScreen = value;
                NotifyPropertyChanged();
            }
        }

        #region Minute display helpers

        public string ShortDisplayTimeFormatted
        {
            get
            {
                return _shortDisplayTimeFormatted;
            }
            set
            {
                _shortDisplayTimeFormatted = value;
                NotifyPropertyChanged();
            }
        }

        public string ShortIntervalTimeFormatted
        {
            get
            {
                return _shortIntervalTimeFormatted;
            }
            set
            {
                _shortIntervalTimeFormatted = value;
                NotifyPropertyChanged();
            }
        }

        public string LongDisplayTimeFormatted
        {
            get
            {
                return _LongDisplayTimeFormatted;
            }
            set
            {
                _LongDisplayTimeFormatted = value;
                NotifyPropertyChanged();
            }
        }

        public string LongIntervalTimeFormatted
        {
            get
            {
                return _longIntervalTimeFormatted;
            }
            set
            {
                _longIntervalTimeFormatted = value;
                NotifyPropertyChanged();
            }
        }
        #endregion
        
        #endregion

        #region Milliseconds getters
        public long GetShortDisplayMillisecond()
        {
            return _shortDisplayTime * 1000;
        }

        public long GetLongDisplayMillisecond()
        {
            return _longDisplayTime * 1000;
        }

        public long GetShortIntervalMillisecond()
        {
            return _shortIntervalTime * 1000;
        }

        public long GetLongIntervalMillisecond()
        {
            return _longIntervalTime * 1000;
        }
        #endregion

        #region Quote getters
        internal string GetShortQuote()
        {
            if (shortBreakQuotes.Count == 0)
            {
                return "Short break";
            }
            else if (shortBreakQuotes.Count == 1)
            {
                return shortBreakQuotes[0];
            }
            else
            {
                int quoteIndex = rand.Next(0, shortBreakQuotes.Count);
                return shortBreakQuotes[quoteIndex];
            }
        }

        internal string GetLongQuote()
        {
            if (longBreakQuotes.Count == 0)
            {
                return "Long break";
            }
            else if (longBreakQuotes.Count == 1)
            {
                return longBreakQuotes[0];
            }
            else
            {
                int quoteIndex = rand.Next(0, longBreakQuotes.Count);
                return longBreakQuotes[quoteIndex];
            }
        }
        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("sdt", _shortDisplayTime, typeof(long));
            info.AddValue("sit", _shortIntervalTime, typeof(long));
            info.AddValue("ldt", _longDisplayTime, typeof(long));
            info.AddValue("lit", _longIntervalTime, typeof(long));

            info.AddValue("iss", _isShortSkippable, typeof(bool));
            info.AddValue("ils", _isLongSkippable, typeof(bool));
            info.AddValue("sbwfs", _shouldBrakeWhenFullScreen, typeof(bool));

            info.AddValue("sbq", shortBreakQuotes.ToArray(), typeof(string[]));
            info.AddValue("lbq", longBreakQuotes.ToArray(), typeof(string[]));
        }
        #endregion

        /// <summary>
        /// Converts the second based times to easily readable format
        /// </summary>
        /// <param name="propertyName"></param>
        private void TimeConverter(string propertyName)
        {
            TimeSpan time;
            switch (propertyName)
            {
                case ("ShortDisplayTime"):
                    time = TimeSpan.FromSeconds(ShortDisplayTime);

                    if (time < TimeSpan.FromSeconds(10))
                    {
                        ShortDisplayTimeFormatted = time.ToString(TOSECONDSHORT);
                    }
                    else if (time < TimeSpan.FromSeconds(60))
                    {
                        ShortDisplayTimeFormatted = time.ToString(TOSECONDLONG);
                    }
                    else if (time < TimeSpan.FromMinutes(10))
                    {
                        ShortDisplayTimeFormatted = time.ToString(TOMINUTESHORT);
                    }
                    else
                    {
                        ShortDisplayTimeFormatted = time.ToString(TOMINUTELONG);
                    }
                    
                    break;

                case ("ShortIntervalTime"):
                    time = TimeSpan.FromSeconds(ShortIntervalTime);

                    if (time < TimeSpan.FromSeconds(60))
                    {
                        ShortIntervalTimeFormatted = time.ToString(TOSECONDLONG);
                    }
                    else if (time < TimeSpan.FromMinutes(10))
                    {
                        ShortIntervalTimeFormatted = time.ToString(TOMINUTESHORT);
                    }
                    else
                    {
                        ShortIntervalTimeFormatted = time.ToString(TOMINUTELONG);
                    }

                    break;

                case ("LongDisplayTime"):
                    time = TimeSpan.FromSeconds(LongDisplayTime);

                    if (time < TimeSpan.FromSeconds(10))
                    {
                        LongDisplayTimeFormatted = time.ToString(TOSECONDSHORT);
                    }
                    else if (time < TimeSpan.FromSeconds(60))
                    {
                        LongDisplayTimeFormatted = time.ToString(TOSECONDLONG);
                    }
                    else if (time < TimeSpan.FromMinutes(10))
                    {
                        LongDisplayTimeFormatted = time.ToString(TOMINUTESHORT);
                    }
                    else
                    {
                        LongDisplayTimeFormatted = time.ToString(TOMINUTELONG);
                    }

                    break;

                case ("LongIntervalTime"):
                    time = TimeSpan.FromSeconds(LongIntervalTime);

                    if (time < TimeSpan.FromMinutes(60))
                    {
                        LongIntervalTimeFormatted = time.ToString(TOMINUTELONG);
                    }
                    else
                    {
                        LongIntervalTimeFormatted = time.ToString(TOHOUR);
                    }

                    break;

                default:
                    break;
            }
        }

    }
}
