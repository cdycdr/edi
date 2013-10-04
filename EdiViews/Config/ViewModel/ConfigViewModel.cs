namespace EdiViews.Config.ViewModel
{
  using EdiViews.ViewModel.Base;
  using Settings.ProgramSettings;
  using SimpleControls.MRU.ViewModel;
  using UnitComboLib.Unit.Screen;
  using UnitComboLib.ViewModel;

  public class ConfigViewModel :  DialogViewModelBase
  {
    #region fields
    private bool mReloadOpenFilesOnAppStart;
    private MRUSortMethod mPinSortMode;
    private bool mRunSingleInstance;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public ConfigViewModel()
    : base()
    {
      this.mReloadOpenFilesOnAppStart = false;
      this.mRunSingleInstance = true;
      this.mPinSortMode = MRUSortMethod.PinnedEntriesFirst;

      var unitList = Settings.SettingsManager.Instance.SettingData.GenerateScreenUnitList();
      this.SizeUnitLabel = new UnitViewModel(unitList, new ScreenConverter(), (int)ZoomUnit.Percentage, 100);
    }
    #endregion constructor

    #region properties
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
    #endregion ScaleView
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

      var unitList = Settings.SettingsManager.Instance.SettingData.GenerateScreenUnitList();
      this.SizeUnitLabel = new UnitViewModel(unitList, new ScreenConverter(),
                                            (int)settingData.DocumentZoomUnit, settingData.DocumentZoomView);
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

      if (this.SizeUnitLabel.SelectedItem.Key == UnitComboLib.Unit.Itemkey.ScreenFontPoints)
        settingData.DocumentZoomUnit = ZoomUnit.Points;
      else
        settingData.DocumentZoomUnit = ZoomUnit.Percentage;

      settingData.DocumentZoomView = (int)this.SizeUnitLabel.Value;
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
