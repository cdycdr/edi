namespace EdiViews.Config.ViewModel
{
  using System;
  using System.IO;
  using System.Xml;
  using System.Xml.Serialization;

  using EdiViews.ViewModel.Base;
  using ICSharpCode.AvalonEdit.Highlighting.Themes;
  using SimpleControls.MRU.ViewModel;
  using Util;
  using System.Reflection;

  [Serializable]
  [XmlRoot(ElementName = "ProgramSettings", IsNullable = false)]
  public class ConfigViewModel : EdiViews.ViewModel.Base.ViewModelBase
  {
    #region fields
    public const int TextEditorZoomInFontSize = 1;
    public const int TextEditorZoomInPercent = 0;

    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private MRUListVM mMruList;
    private HlThemeKey mCurrentTheme;
    private SerializableDictionary<HlThemeKey, HlThemeConfig> mHlThemesConfig = null;
    #endregion fields

    #region constructor
    public ConfigViewModel()
    {
      this.mCurrentTheme = new HlThemeKey(EdiThemesViewModel.DefaultWPFTheme);

      this.DocumentZoomUnit = TextEditorZoomInPercent;     // Zoom View in Percent
      this.DocumentZoomView = 150;                        // Font Size 12 is 100 %
      this.ReloadOpenFilesOnAppStart = true;
      this.RunSingleInstance = true;
      
      // Session Data
      this.MainWindowPosSz = new ViewPosSzViewModel();
      this.MainWindowPosSz = new ViewPosSzViewModel(100,100, 1000, 700);

      this.LastActiveFile = string.Empty;

      // Construct MRUListVM ViewModel with parameter to decide whether pinned entries
      // are sorted into the first (pinned list) spot or not (favourites list)
      this.mMruList = new MRUListVM(MRUSortMethod.PinnedEntriesFirst);

      // ExpressionDark has a default highlighting theme since most highlightings
      // are really designed to work for black on white (and not visce versa)
      this.mHlThemesConfig = new SerializableDictionary<HlThemeKey, HlThemeConfig>();

      string appLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

      string themeName = "Deep Black";
      HlThemeConfig hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\DeepBlack.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.ExpressionDark, themeName) { HlThemeName = "Expression Dark (Deep Black)" }, hlTheme);

      hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\DeepBlack.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.ExpressionDark2, themeName) { HlThemeName = "Expression Dark 2 (Deep Black)" }, hlTheme);

      themeName = "Bright Standard";
      hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\BrightStandard.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.VS2010, themeName) { HlThemeName = "VS2010 (Bright Standard)" }, hlTheme);

      hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\BrightStandard.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.Aero, themeName) { HlThemeName = "Aero (Bright Standard)" }, hlTheme);

      hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\BrightStandard.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.Generic, themeName) { HlThemeName = "Generic (Bright Standard)" }, hlTheme);

      hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\BrightStandard.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.ExpressionLight2, themeName) { HlThemeName = "Expression Light 2 (Bright Standard)" }, hlTheme);

      hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\BrightStandard.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.Metro, themeName) { HlThemeName = "Metro (Bright Standard)" }, hlTheme);

      themeName = "True Blue";

      hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\TrueBlue.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.ExpressionDark2, themeName) { HlThemeName = "Expression Dark 2 (True Blue)" }, hlTheme);

      hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\TrueBlue.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.ExpressionLight2, themeName) { HlThemeName = "Expression Light 2 (True Blue)" }, hlTheme);

      hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\TrueBlue.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.ExpressionDark, themeName) { HlThemeName = "Expression Dark (True Blue)" }, hlTheme);

      hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\TrueBlue.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.VS2010, themeName) { HlThemeName = "VS2010 (TrueBlue)" }, hlTheme);

      hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\TrueBlue.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.Aero, themeName) { HlThemeName = "Aero (True Blue)" }, hlTheme);

      hlTheme = new HlThemeConfig(themeName, appLocation, @"AvalonEdit\HighLighting_Themes\TrueBlue.xshd");
      this.mHlThemesConfig.Add(new HlThemeKey(EdiThemesViewModel.WPFTheme.Metro, themeName) { HlThemeName = "Metro (True Blue)" }, hlTheme);
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Percentage Size of data to be viewed by default
    /// </summary>
    [XmlAttribute(AttributeName = "DocumentZoomView")]
    public int DocumentZoomView { get; set; }

    [XmlAttribute(AttributeName = "DocumentZoomUnit")]
    public int DocumentZoomUnit { get; set; }

    /// <summary>
    /// Get/set whether application re-loads files open in last sesssion or not
    /// </summary>
    [XmlAttribute(AttributeName = "ReloadOpenFilesFromLastSession")]
    public bool ReloadOpenFilesOnAppStart { get; set; }

    #region Session Data
    /// <summary>
    /// Get/set position and size of MainWindow
    /// </summary>
    [XmlElement(ElementName = "MainWindowPos")]
    public ViewPosSzViewModel MainWindowPosSz { get; set; }

    [XmlElement(ElementName = "RunSingleInstance")]
    public bool RunSingleInstance { get; set; }

    /// <summary>
    /// Remember the last active path and name of last active document.
    /// 
    /// This can be useful when selecting active document in next session or
    /// determining a useful default path when there is no document currently open.
    /// </summary>
    [XmlAttribute(AttributeName = "LastActiveFile")]
    public string LastActiveFile { get; set; }

    #region theming
    /// <summary>
    /// Get/set WPF theme for the complete Application
    /// </summary>
    [XmlElement("CurrentTheme")]
    public HlThemeKey CurrentTheme
    {
      get
      {
        return this.mCurrentTheme;
      }

      set
      {
        if (this.mCurrentTheme != value)
        {
          this.mCurrentTheme = value;    // OnPropertChanged is part of this call

          if (ThemeChanged != null)
            ThemeChanged(this, EventArgs.Empty);
        }
      }
    }

    [XmlElement("HighlightingThemes")]
    public SerializableDictionary<HlThemeKey, HlThemeConfig> HlThemesConfig
    {
      get
      {
        if (this.mHlThemesConfig == null)
          this.mHlThemesConfig = new SerializableDictionary<HlThemeKey, HlThemeConfig>();

        return mHlThemesConfig;
      }

      set
      {
        if (this.mHlThemesConfig != value)
        {
          this.mHlThemesConfig = value;

          this.NotifyPropertyChanged(() => this.mHlThemesConfig);
        }
      }
    }

    [XmlIgnore]
    public EventHandler ThemeChanged = null;
    #endregion theming

    /// <summary>
    /// List of most recently used files
    /// </summary>
    public MRUListVM MruList
    {
      get
      {
        if (this.mMruList == null)
          this.mMruList = new MRUListVM();

        return this.mMruList;
      }

      set
      {
        if (this.mMruList != value)
        {
          this.mMruList = value;
          this.NotifyPropertyChanged(() => this.MruList);
        }
      }
    }
    #endregion Session Data
    #endregion properties

    #region methods
    /// <summary>
    /// Get the path of the file or empty string if file does not exists on disk.
    /// </summary>
    /// <returns></returns>
    public string GetLastActivePath()
    {
      try
      {
        if (System.IO.File.Exists(this.LastActiveFile))
          return System.IO.Path.GetDirectoryName(this.LastActiveFile);
      }
      catch
      {
      }

      return string.Empty;
    }

    /// <summary>
    /// Determine whether program options are valid and corrext
    /// settings if they appear to be invalid on current system
    /// </summary>
    internal void CheckSettingsOnLoad()
    {
      if (MainWindowPosSz == null)
        MainWindowPosSz = new ViewPosSzViewModel(100,100,600, 500);

      if (MainWindowPosSz.DefaultConstruct == true)
        MainWindowPosSz = new ViewPosSzViewModel(100, 100, 600, 500);

      MainWindowPosSz.SetValidPos();
    }

    #region Load Save ProgramOptions ViewModel
    /// <summary>
    /// Save program options into persistence.
    /// See <seealso cref="SaveOptions"/> to save program options on program end.
    /// </summary>
    /// <param name="settingsFileName"></param>
    /// <returns></returns>
    public static ConfigViewModel LoadOptions(string settingsFileName)
    {
      ConfigViewModel loadedViewModel = null;

      if (System.IO.File.Exists(settingsFileName))
      {
        // Create a new file stream for reading the XML file
        using (FileStream readFileStream = new System.IO.FileStream(settingsFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
          try
          {
            // Create a new XmlSerializer instance with the type of the test class
            XmlSerializer serializerObj = new XmlSerializer(typeof(ConfigViewModel));

            // Load the object saved above by using the Deserialize function
            loadedViewModel = (ConfigViewModel)serializerObj.Deserialize(readFileStream);
          }
          catch (Exception e)
          {
            logger.Error(e);

            loadedViewModel = new ConfigViewModel();  // Just get the defaults if serilization wasn't working here...
          }

          // Cleanup
          readFileStream.Close();
        }
      }

      return loadedViewModel;
    }

    /// <summary>
    /// Save program options into persistence.
    /// See <seealso cref="LoadOptions"/> to load program options on program start.
    /// </summary>
    /// <param name="settingsFileName"></param>
    /// <param name="vm"></param>
    /// <returns></returns>
    public static bool SaveOptions(string settingsFileName, ConfigViewModel vm)
    {
      try
      {
        XmlWriterSettings xws = new XmlWriterSettings();
        xws.NewLineOnAttributes = true;
        xws.Indent = true;
        xws.IndentChars = "  ";
        xws.Encoding = System.Text.Encoding.UTF8;

        // Create a new file stream to write the serialized object to a file
        using (XmlWriter xw = XmlWriter.Create(settingsFileName, xws))
        {
          // Create a new XmlSerializer instance with the type of the test class
          XmlSerializer serializerObj = new XmlSerializer(typeof(ConfigViewModel));

          serializerObj.Serialize(xw, vm);

          xw.Close(); // Cleanup

          return true;
        }
      }
      catch
      {
        throw;
      }
    }
    #endregion Load Save ProgramOptions ViewModel

    /// <summary>
    /// Check whether the <paramref name="WpfTheme"/> is configured
    /// with a highlighting theme and return it if that is the case.
    /// </summary>
    /// <param name="WpfTheme"></param>
    /// <returns>List of highlighting themes that should be applied for this WPF theme</returns>
    public HighlightingThemes FindHighlightingTheme(HlThemeKey hlTheme)
    {
      // Is this WPF theme configured with a highlighting theme???
      HlThemeConfig cfg = null;

      this.HlThemesConfig.TryGetValue(hlTheme, out cfg);

      if (cfg != null)
        return cfg.HighlightingStyles;

      return null;
    }
    #endregion methods
  }
}
