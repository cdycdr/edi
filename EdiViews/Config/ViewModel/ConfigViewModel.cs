namespace EdiViews.Config.ViewModel
{
  using System.Collections.ObjectModel;
  using System.Linq;
  using EdiViews.ViewModel.Base;
  using ICSharpCode.AvalonEdit;
  using Settings.ProgramSettings;
  using SimpleControls.MRU.ViewModel;
  using UnitComboLib.Unit.Screen;
  using UnitComboLib.ViewModel;

  public class ConfigViewModel :  DialogViewModelBase
  {
    #region fields
    private bool mWordWrapText;
    private bool mReloadOpenFilesOnAppStart;
    private MRUSortMethod mPinSortMode;
    private bool mRunSingleInstance;

    private LanguageCollection mLanguageSelected;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public ConfigViewModel()
    : base()
    {
      // Setup default values here - real values are loaded in a specific method of this class (!)
      this.mWordWrapText = false;
      this.mReloadOpenFilesOnAppStart = false;
      this.mRunSingleInstance = true;
      this.mPinSortMode = MRUSortMethod.PinnedEntriesFirst;

      this.WordWrapText = false;

      // Get default list of units from settings manager
      var unitList = new ObservableCollection<ListItem>(Options.GenerateScreenUnitList());
      this.SizeUnitLabel = new UnitViewModel(unitList, new ScreenConverter(), (int)ZoomUnit.Percentage, 100);

      this.EditorTextOptions = new TextEditorOptions();

      // Initialize localization settings
      this.Languages = new ObservableCollection<LanguageCollection>(Options.GetSupportedLanguages());

      // Set default language to make sure app neutral is selected and available for sure
      // (this is a fallback if all else fails)
      try
      {
        this.LanguageSelected = this.Languages.FirstOrDefault(lang => lang.BCP47 == Options.DefaultLocal);
      }
      catch
      {
      }
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set whether WordWarp should be applied in editor (by default) or not.
    /// </summary>
    public bool WordWrapText
    {
      get
      {
        return this.mWordWrapText;
      }

      set
      {
        if (this.mWordWrapText != value)
        {
          this.mWordWrapText = value;
          this.NotifyPropertyChanged(() => this.WordWrapText);
        }
      }
    }

    /// <summary>
    /// Expose AvalonEdit Text Editing options for editing in program settings view.
    /// </summary>
    public TextEditorOptions EditorTextOptions { get; set; }

    /// <summary>
    /// Get/set MRU pin sort mode to determine MRU pin behaviour.
    /// </summary>
    public MRUSortMethod MruPinSortMode
    {
      get
      {
        return this.mPinSortMode;
      }

      set
      {
        if (this.mPinSortMode != value)
        {
          this.mPinSortMode = value;
          this.NotifyPropertyChanged(() => this.MruPinSortMode);
        }
      }
    }

    /// <summary>
    /// Get/set whether application re-loads files open in last sesssion or not
    /// </summary>
    public bool ReloadOpenFilesOnAppStart
    {
      get
      {
        return this.mReloadOpenFilesOnAppStart;
      }

      set
      {
        if (this.mReloadOpenFilesOnAppStart != value)
        {
          this.mReloadOpenFilesOnAppStart = value;
          this.NotifyPropertyChanged(() => this.ReloadOpenFilesOnAppStart);
        }
      }
    }

    /// <summary>
    /// Get/set whether application can be started more than once.
    /// </summary>
    public bool RunSingleInstance
    {
      get
      {
        return this.mRunSingleInstance;
      }

      set
      {
        if (this.mRunSingleInstance != value)
        {
          this.mRunSingleInstance = value;
          this.NotifyPropertyChanged(() => this.RunSingleInstance);
        }
      }
    }

    #region ScaleView
    /// <summary>
    /// Scale view of text in percentage of font size
    /// </summary>
    public UnitViewModel SizeUnitLabel { get; set; }

    /// <summary>
    /// Get/set unit of document zoom unit.
    /// </summary>
    public ZoomUnit DocumentZoomUnit
    {
      get
      {
        if (this.SizeUnitLabel.SelectedItem.Key == UnitComboLib.Unit.Itemkey.ScreenFontPoints)
          return ZoomUnit.Points;

        return ZoomUnit.Percentage;
      }

      set
      {
        if (ConvertZoomUnit(value) != this.SizeUnitLabel.SelectedItem.Key)
        {
          if (value == ZoomUnit.Points)
            this.SizeUnitLabel.SetSelectedItemCommand.Execute(UnitComboLib.Unit.Itemkey.ScreenFontPoints);
          else
            this.SizeUnitLabel.SetSelectedItemCommand.Execute(UnitComboLib.Unit.Itemkey.ScreenPercent);

          this.NotifyPropertyChanged(() => this.DocumentZoomUnit);
        }
      }
    }

    /// <summary>
    /// Get the title string of the view - to be displayed in the associated view
    /// (e.g. as dialog title)
    /// </summary>
    public string WindowTitle
    {
      get
      {
        return Util.Local.Strings.STR_ProgramSettings_Caption;
      }
    }
    #endregion ScaleView

    #region Language Localization Support
    /// <summary>
    /// Get list of GUI languages supported in this application.
    /// </summary>
    public ObservableCollection<LanguageCollection> Languages { get; private set; }

    /// <summary>
    /// Get/set language of message box buttons for display in localized form.
    /// </summary>
    public LanguageCollection LanguageSelected
    {
      get
      {
        return this.mLanguageSelected;
      }

      set
      {
        if (this.mLanguageSelected != value)
        {
          this.mLanguageSelected = value;
          this.NotifyPropertyChanged(() => this.LanguageSelected);
        }
      }
    }
    #endregion Language Localization Support
    #endregion properties

    #region methods
    /// <summary>
    /// Reset the view model to those options that are going to be presented for editing.
    /// </summary>
    /// <param name="settingData"></param>
    public void LoadOptionsFromModel(Options settingData)
    {
      // Load Mru Options from model
      this.MruPinSortMode = settingData.MRU_SortMethod;

      this.ReloadOpenFilesOnAppStart = settingData.ReloadOpenFilesOnAppStart;
      this.RunSingleInstance = settingData.RunSingleInstance;

      this.WordWrapText = settingData.WordWarpText;

      this.EditorTextOptions = new TextEditorOptions(settingData.EditorTextOptions);
      this.SizeUnitLabel = new UnitViewModel(new ObservableCollection<ListItem>(Options.GenerateScreenUnitList()),
                                             new ScreenConverter(),
                                            (int)settingData.DocumentZoomUnit, settingData.DocumentZoomView);

      try
      {
        this.LanguageSelected = this.Languages.FirstOrDefault(lang => lang.BCP47 == settingData.LanguageSelected);
      }
      catch
      {
      }
    }

    /// <summary>
    /// Save changed settings back to model for further
    /// application and persistence in file system.
    /// </summary>
    /// <param name="settingData"></param>
    public void SaveOptionsToModel(Options settingData)
    {
      settingData.MRU_SortMethod = this.MruPinSortMode;
      settingData.ReloadOpenFilesOnAppStart = this.ReloadOpenFilesOnAppStart;
      settingData.RunSingleInstance = this.RunSingleInstance;

      settingData.WordWarpText = this.WordWrapText;

      settingData.EditorTextOptions = new TextEditorOptions(this.EditorTextOptions);
      if (this.SizeUnitLabel.SelectedItem.Key == UnitComboLib.Unit.Itemkey.ScreenFontPoints)
        settingData.DocumentZoomUnit = ZoomUnit.Points;
      else
        settingData.DocumentZoomUnit = ZoomUnit.Percentage;

      settingData.DocumentZoomView = (int)this.SizeUnitLabel.Value;

      settingData.LanguageSelected = this.LanguageSelected.BCP47;

      settingData.IsDirty = true;
    }

    /// <summary>
    /// Convert between local zoom unit enumeration and remote zoom unit enumeration.
    /// </summary>
    /// <param name="unit"></param>
    /// <returns></returns>
    private UnitComboLib.Unit.Itemkey ConvertZoomUnit(ZoomUnit unit)
    {
      switch (unit)
      {
        case ZoomUnit.Percentage:
          return UnitComboLib.Unit.Itemkey.ScreenPercent;

        case ZoomUnit.Points:
          return UnitComboLib.Unit.Itemkey.ScreenFontPoints;

        default:
          throw new System.NotImplementedException(unit.ToString());
      }
    }
    #endregion methods
  }
}
