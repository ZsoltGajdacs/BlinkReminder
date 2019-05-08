using BlinkReminder.DTOs;
using BlinkReminder.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;

namespace BlinkReminder.Settings
{
    /// <summary>
    /// Keeps the current settings
    /// </summary>
    [Serializable]
    internal class UserSettings : INotifyPropertyChanged, ISerializable
    {
        #region Data members
        // Logger
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // Settings data
        private const string SETTINGS_FILENAME = "Settings.brs";
        internal string SettingsFilePath { get; set; }
        internal string SettingsDirPath { get; set; }

        // For keeping the user set times
        private TimeSpan _shortDisplayTime;
        private TimeSpan _shortIntervalTime;
        private TimeSpan _longDisplayTime;
        private TimeSpan _longIntervalTime;

        // For locked screen length intrepretation
        private TimeSpan _lockLengthTimeExtent;

        // For setting whether the breaks are skippable
        private bool _isShortSkippable;
        private bool _isLongSkippable;

        // For checking if breaks should occur when there is a fullscreen app running
        private bool _shouldBrakeWhenFullScreen;

        // For indefinite pause enablement
        private bool _indefPauseEnabled;

        // For setting full screen vs small break screen
        private bool _isFullscreenBreak;

        // User scaling for small screen break
        private double _scaling;

        // For keeping the quotes that appear during breaks
        private BindingList<Quote> _shortBreakQuotes;
        private BindingList<Quote> _longBreakQuotes;

        // For selection which quote to show
        private RandIntMem rand;

        // DTO for communicating with windows
        [field: NonSerializedAttribute()]
        public SettingsDTO SettingsDTO { get; set; }

        // Event handler for MVVM support
        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Singleton stuff
        private static readonly Lazy<UserSettings> lazy = new Lazy<UserSettings>(() =>
        {
            string settingsPath = GetSettingsLocation();
            UserSettings settings = (UserSettings)Serializer.DeserializeObject(settingsPath);
            
            return settings ?? new UserSettings();
        });

        public static UserSettings Instance { get { return lazy.Value; } }
        #endregion

        #region Constructors
        /// <summary>
        /// Default ctor sets default values
        /// </summary>
        private UserSettings()
        {
            SetSettingsDirLocation();
            SettingsFilePath = GetSettingsLocation();

            ShortDisplayTime = TimeSpan.FromMilliseconds((double)CycleTimesEnum.ShortDisplayTime);
            ShortIntervalTime = TimeSpan.FromMilliseconds((double)CycleTimesEnum.ShortIntervalTime);
            LongDisplayTime = TimeSpan.FromMilliseconds((double)CycleTimesEnum.LongDisplayTime);
            LongIntervalTime = TimeSpan.FromMilliseconds((double)CycleTimesEnum.LongIntervalTime);

            LockLengthTimeExtent = TimeSpan.FromMinutes(1);

            IsShortSkippable = false;
            IsLongSkippable = true;
            ShouldBreakWhenFullScreen = true;
            IndefPauseEnabled = false;
            IsFullscreenBreak = true;

            Scaling = 1;

            AddDefaultQuotes();
            CreateStuff();

            logger.Info("New settings created with defaults");
        }

        /// <summary>
        /// Serialization ctor, loads data from file
        /// </summary>
        private UserSettings(SerializationInfo info, StreamingContext context)
        {
            SetSettingsDirLocation();
            SettingsFilePath = GetSettingsLocation();

            // ------------------- v0.5 settings -------------------
            IsShortSkippable = (bool)info.GetValue("iss", typeof(bool));
            IsLongSkippable = (bool)info.GetValue("ils", typeof(bool));
            ShouldBreakWhenFullScreen = (bool)info.GetValue("sbwfs", typeof(bool));

            _shortBreakQuotes = new BindingList<Quote>(((Quote[])info.GetValue("sbq", typeof(Quote[]))).ToList());
            _longBreakQuotes = new BindingList<Quote>(((Quote[])info.GetValue("lbq", typeof(Quote[]))).ToList());

            // Settings options after v0.5 must go in "try" blocks as they might be missing from the 
            // file on the user's end.
            //--------------------- v0.6 settings --------------------
            try
            {
                IndefPauseEnabled = (bool)info.GetValue("ipe", typeof(bool));
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Getting IndefPauseEnabled failed");
                IndefPauseEnabled = false;
            }

            //--------------------- v0.7 settings --------------------
            try
            {
                ShortDisplayTime = (TimeSpan)info.GetValue("sdt", typeof(TimeSpan));
                ShortIntervalTime = (TimeSpan)info.GetValue("sit", typeof(TimeSpan));
                LongDisplayTime = (TimeSpan)info.GetValue("ldt", typeof(TimeSpan));
                LongIntervalTime = (TimeSpan)info.GetValue("lit", typeof(TimeSpan));

                LockLengthTimeExtent = (TimeSpan)info.GetValue("llte", typeof(TimeSpan));

                IsFullscreenBreak = (bool)info.GetValue("ifb", typeof(bool));

                Scaling = (double)info.GetValue("scl", typeof(double));
            }
            catch (Exception ex)
            {
                logger.Debug(ex, "Getting 0.7 settings failed");

                ShortDisplayTime = TimeSpan.FromSeconds((double)info.GetValue("sdt", typeof(double)));
                ShortIntervalTime = TimeSpan.FromSeconds((double)info.GetValue("sit", typeof(double)));
                LongDisplayTime = TimeSpan.FromSeconds((double)info.GetValue("ldt", typeof(double)));
                LongIntervalTime = TimeSpan.FromSeconds((double)info.GetValue("lit", typeof(double)));

                LockLengthTimeExtent = TimeSpan.FromMinutes(1);

                IsFullscreenBreak = true;

                Scaling = 1;
            }

            CreateStuff(false);

            logger.Info("Settings successfully deserialzed");
        }
        #endregion

        #region Static methods

        /// <summary>
        /// Gives back the path of the settings file.
        /// </summary>
        /// <returns></returns>
        private static string GetSettingsLocation()
        {
            string userAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return userAppData + "\\BlinkReminder\\" + SETTINGS_FILENAME;
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
                new Quote("Look out the window", true, true),
                new Quote("Stretch your legs", true, true),
                new Quote("Close your eyes", true, true),
                new Quote("Drink some water", true, true)
            };

            _longBreakQuotes = new BindingList<Quote>()
            {
                new Quote("Take a bit of walk", true, false),
                new Quote("Have a cup of tea/coffee", true, false),
                new Quote("Relax", true, false)
            };
        }

        /// <summary>
        /// Creates the necessary custom data members
        /// </summary>
        private void CreateStuff(bool isNew = true)
        {
            SettingsDTO = new SettingsDTO(ref _shortDisplayTime, ref _shortIntervalTime,
                                            ref _longDisplayTime, ref _longIntervalTime,
                                            ref _lockLengthTimeExtent);

            SettingsDTO.UserInactivityTimer.Elapsed += UserInactivityTimer_Elapsed;

            if (isNew)
            {
                rand = new RandIntMem(2);
            }
            else
            {
                rand = new RandIntMem(1);
            }

            // Check the amount of quotes to know how many can be shown without repetition
            // Commented out until flexible RandIntMem is implemented
            /*if (_shortBreakQuotes.Count >= 3 && _longBreakQuotes.Count >= 3)
            {
                rand = new RandIntMem(2);
            }
            else
            {
                rand = new RandIntMem(1);
            }*/

        }

        #endregion

        #region Property changed handler

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        #region Accessor properties

        #region Time setters

        public TimeSpan ShortDisplayTime
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
        public TimeSpan ShortIntervalTime
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

        public TimeSpan LongDisplayTime
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
        public TimeSpan LongIntervalTime
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

        #region Booleans
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

        public bool IndefPauseEnabled
        {
            get
            {
                return _indefPauseEnabled;
            }

            set
            {
                _indefPauseEnabled = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsFullscreenBreak
        {
            get
            {
                return _isFullscreenBreak;
            }

            set
            {
                if (value != _isFullscreenBreak)
                {
                    _isFullscreenBreak = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Others
        public double Scaling
        {
            get
            {
                return _scaling;
            }

            set
            {
                if (value != _scaling)
                {
                    _scaling = value;
                }
            }
        }
        
        /// <summary>
        /// The amount of time below which a locked screen is considered a short break 
        /// or above it a long one
        /// </summary>
        public TimeSpan LockLengthTimeExtent
        {
            get
            {
                return _lockLengthTimeExtent;
            }

            set
            {
                if (value != _lockLengthTimeExtent)
                {
                    _lockLengthTimeExtent = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #endregion

        #region Quote getters
        /// <summary>
        /// Gives back a randomly chosen short quote
        /// </summary>
        /// <returns></returns>
        internal string GetShortQuote()
        {
            string defaultShortQuote = "Short break";

            return GetQuote(ref _shortBreakQuotes, defaultShortQuote);
        }

        /// <summary>
        /// Gives back a randomly chosen long quote
        /// </summary>
        /// <returns></returns>
        internal string GetLongQuote()
        {
            string defaultLongQuote = "Long break";

            return GetQuote(ref _longBreakQuotes, defaultLongQuote);
        }
        #endregion

        #region Event Handler

        private void UserInactivityTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            ShortDisplayTime = TimeSpan.FromSeconds(SettingsDTO.ShortDisplayAmount);
            ShortIntervalTime = TimeSpan.FromSeconds(SettingsDTO.ShortIntervalAmount);
            LongDisplayTime = TimeSpan.FromSeconds(SettingsDTO.LongDisplayAmount);
            LongIntervalTime = TimeSpan.FromSeconds(SettingsDTO.LongIntervalAmount);
            LockLengthTimeExtent = TimeSpan.FromMinutes(SettingsDTO.LockLengthTimeExtent);
        }

        #endregion

        #region Serialization
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("sdt", _shortDisplayTime, typeof(TimeSpan));
            info.AddValue("sit", _shortIntervalTime, typeof(TimeSpan));
            info.AddValue("ldt", _longDisplayTime, typeof(TimeSpan));
            info.AddValue("lit", _longIntervalTime, typeof(TimeSpan));

            info.AddValue("llte", _lockLengthTimeExtent, typeof(TimeSpan));

            info.AddValue("iss", _isShortSkippable, typeof(bool));
            info.AddValue("ils", _isLongSkippable, typeof(bool));
            info.AddValue("sbwfs", _shouldBrakeWhenFullScreen, typeof(bool));
            info.AddValue("ipe", _indefPauseEnabled, typeof(bool));
            info.AddValue("ifb", _isFullscreenBreak, typeof(bool));

            info.AddValue("scl", _scaling, typeof(double));

            info.AddValue("sbq", _shortBreakQuotes.ToArray(), typeof(Quote[]));
            info.AddValue("lbq", _longBreakQuotes.ToArray(), typeof(Quote[]));
        }
        #endregion

        #region Helpers

        /// <summary>
        /// Gives back a randomly chosen quote from the given list
        /// </summary>
        /// <param name="quoteList"></param>
        /// <returns></returns>
        private string GetQuote(ref BindingList<Quote> quoteList, string defaultQuote)
        {
            if (quoteList.Count == 0)
            {
                return defaultQuote;
            }
            else if (quoteList.Count == 1)
            {
                if (quoteList[0].IsActive)
                {
                    return quoteList[0].QuoteText;
                }
                else
                {
                    return defaultQuote;
                }
            }
            else
            {
                int quoteIndex;

                do
                {
                    quoteIndex = rand.GetRandInt(0, quoteList.Count - 1);
                } while (!quoteList[quoteIndex].IsActive);

                return quoteList[quoteIndex].QuoteText;
            }
        }

        /// <summary>
        /// Sets the application settings directory property
        /// </summary>
        private void SetSettingsDirLocation()
        {
            string userAppData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            SettingsDirPath = userAppData + "\\BlinkReminder";
        }

        #endregion
    }
}
