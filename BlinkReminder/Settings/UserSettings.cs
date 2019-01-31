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
        #region Data members
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
        private BindingList<Quote> _shortBreakQuotes;
        private BindingList<Quote> _longBreakQuotes;

        // For checking if breaks should occur when there is a fullscreen app running
        private bool _shouldBrakeWhenFullScreen;

        // For selection which quote to show
        private Random rand;

        // Event handler for MVVM support
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Singleton stuff
        private static readonly Lazy<UserSettings> lazy = new Lazy<UserSettings>(() =>
        {
            if (File.Exists(SETTINGSFILEPATH) && new FileInfo(SETTINGSFILEPATH).Length > 0)
            {
                try
                {
                    IFormatter formatter = new BinaryFormatter();
                    FileStream stream = new FileStream(SETTINGSFILEPATH, FileMode.Open, FileAccess.Read);
                    return (UserSettings)formatter.Deserialize(stream);
                }
                catch (Exception)
                {
                    return new UserSettings();
                }
            }
            else
            {
                return new UserSettings();
            }


        });

        public static UserSettings Instance { get { return lazy.Value; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Default ctor sets defaul values
        /// </summary>
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

            AddDefaultQuotes();

            rand = new Random();
        }

        /// <summary>
        /// Serialization ctor, loads data from file
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
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

            _shortBreakQuotes = new BindingList<Quote>(((Quote[])info.GetValue("sbq", typeof(Quote[]))).ToList());
            _longBreakQuotes = new BindingList<Quote>(((Quote[])info.GetValue("lbq", typeof(Quote[]))).ToList());

            rand = new Random();
        }
        #endregion

        #region Startup methods
        /// <summary>
        /// Adds default quotes to the long and short break lists
        /// </summary>
        private void AddDefaultQuotes()
        {
            _shortBreakQuotes = new BindingList<Quote>()
            {
                new Quote("Look out the window", true, 0),
                new Quote("Strech your legs", true, 1),
                new Quote("Close your eyes", true, 2),
                new Quote("Drink some water", true, 3)
            };

            _longBreakQuotes = new BindingList<Quote>()
            {
                new Quote("Take a bit of walk", true, 0),
                new Quote("Have a cup of tea/coffee", true, 1),
                new Quote("Relax", true, 2)
            };
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

        #region Quotes

        public BindingList<Quote> ShortBreakQuotes
        {
            get
            {
                return _shortBreakQuotes;
            }
        }

        public BindingList<Quote> LongBreakQuotes
        {
            get
            {
                return _longBreakQuotes;
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
            if (_shortBreakQuotes.Count == 0)
            {
                return "Short break";
            }
            else if (_shortBreakQuotes.Count == 1)
            {
                return _shortBreakQuotes[0].QuoteText;
            }
            else
            {
                int quoteIndex = rand.Next(0, _shortBreakQuotes.Count);
                return _shortBreakQuotes[quoteIndex].QuoteText;
            }
        }

        internal string GetLongQuote()
        {
            if (_longBreakQuotes.Count == 0)
            {
                return "Long break";
            }
            else if (_longBreakQuotes.Count == 1)
            {
                return _longBreakQuotes[0].QuoteText;
            }
            else
            {
                int quoteIndex = rand.Next(0, _longBreakQuotes.Count);
                return _longBreakQuotes[quoteIndex].QuoteText;
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

            info.AddValue("sbq", _shortBreakQuotes.ToArray(), typeof(Quote[]));
            info.AddValue("lbq", _longBreakQuotes.ToArray(), typeof(Quote[]));
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
