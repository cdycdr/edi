namespace Settings.ProgramSettings
{
  using System;
  using System.Collections.ObjectModel;
  using System.Xml.Serialization;
  using ICSharpCode.AvalonEdit.Highlighting.Themes;
  using SimpleControls.MRU.ViewModel;
  using Themes;
  using UnitComboLib.Unit;
  using UnitComboLib.ViewModel;

  public enum ZoomUnit
  {
    Percentage = 0,
    Points = 1
  }

  /// <summary>
  /// This class implements the model of the programm settings part
  /// of the application. Typically, users have options that the want
  /// to set or reset durring the live time of an application. This
  /// class organizes these options and is responsible for their
  /// storage (when being changed) and retrieval at program start-up.
  /// </summary>
  [Serializable]
  [XmlRoot(ElementName = "Options", IsNullable = false)]
  public class Options
  {
    #region fields
    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private string mCurrentTheme;

    private int mDocumentZoomView;
    private ZoomUnit mDocumentZoomUnit;

    private bool mReloadOpenFilesOnAppStart;
    private bool mRunSingleInstance;

    private MRUSortMethod mMRU_SortMethod;

    private bool mIsDirty = false;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class Constructor
    /// </summary>
    public Options()
    {
      this.mCurrentTheme = ThemesManager.DefaultThemeName;

      this.mDocumentZoomUnit = ZoomUnit.Percentage;     // Zoom View in Percent
      this.mDocumentZoomView = 100;                     // Font Size 12 is 100 %

      this.mReloadOpenFilesOnAppStart = true;
      this.mRunSingleInstance = true;

      this.mMRU_SortMethod = MRUSortMethod.PinnedEntriesFirst;

      this.mIsDirty = false;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copyThis"></param>
    public Options(Options copyThis)
    : this()
    {
      if (copyThis == null)
        return;

      this.mCurrentTheme = copyThis.mCurrentTheme;

      this.mDocumentZoomUnit = copyThis.mDocumentZoomUnit;     // Zoom View in Percent
      this.mDocumentZoomView = copyThis.mDocumentZoomView;     // Font Size 12 is 100 %

      this.mReloadOpenFilesOnAppStart = copyThis.mReloadOpenFilesOnAppStart;
      this.mRunSingleInstance = copyThis.mRunSingleInstance;

      this.mMRU_SortMethod = copyThis.mMRU_SortMethod;

      this.mIsDirty = copyThis.mIsDirty;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Percentage Size of data to be viewed by default
    /// </summary>
    [XmlAttribute(AttributeName = "DocumentZoomView")]
    public int DocumentZoomView
    {
      get
      {
        return this.mDocumentZoomView;
      }

      set
      {
        if (this.mDocumentZoomView != value)
        {
          this.mDocumentZoomView = value;
          this.IsDirty = true;
        }
      }
    }

    [XmlAttribute(AttributeName = "DocumentZoomUnit")]
    public ZoomUnit DocumentZoomUnit
    {
      get
      {
        return this.mDocumentZoomUnit;
      }

      set
      {
        if (this.mDocumentZoomUnit != value)
        {
          this.mDocumentZoomUnit = value;
          this.IsDirty = true;
        }
      }
    }

    /// <summary>
    /// Get/set whether application re-loads files open in last sesssion or not
    /// </summary>
    [XmlAttribute(AttributeName = "ReloadOpenFilesFromLastSession")]
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
          this.IsDirty = true;
        }
      }
    }

    /// <summary>
    /// Get/set whether application can be started more than once.
    /// </summary>
    [XmlElement(ElementName = "RunSingleInstance")]
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
          this.IsDirty = true;
        }
      }
    }

    /// <summary>
    /// Get/set WPF theme configured for the complete Application
    /// </summary>
    [XmlElement("CurrentTheme")]
    public string CurrentTheme
    {
      get
      {
        return this.mCurrentTheme;
      }

      set
      {
        if (this.mCurrentTheme != value)
        {
          this.mCurrentTheme = value;
          this.IsDirty = true;
        }
      }
    }

    [XmlElement("MRU_SortMethod")]
    public MRUSortMethod MRU_SortMethod
    {
      get
      {
        return this.mMRU_SortMethod;
      }

      set
      {
        if (this.mMRU_SortMethod != value)
        {
          this.mMRU_SortMethod = value;
          this.IsDirty = true;
        }
      }
    }

    [XmlIgnore]
    public bool IsDirty
    {
      get
      {
        return this.mIsDirty;
      }

      private set
      {
        if (this.mIsDirty != value)
          this.mIsDirty = value;
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Check whether the <paramref name="hlThemeName"/> is configured
    /// with a highlighting theme and return it if that is the case.
    /// </summary>
    /// <param name="hlThemeName"></param>
    /// <returns>List of highlighting themes that should be applied for this WPF theme</returns>
    public HighlightingThemes FindHighlightingTheme(string hlThemeName)
    {
      return ThemesManager.Instance.GetTextEditorHighlighting(hlThemeName);
    }

    /// <summary>
    /// Initialize Scale View with useful units in percent and font point size
    /// </summary>
    /// <returns></returns>
    public ObservableCollection<ListItem> GenerateScreenUnitList()
    {
      ObservableCollection<ListItem> unitList = new ObservableCollection<ListItem>();

      var percentDefaults = new ObservableCollection<string>() { "25", "50", "75", "100", "125", "150", "175", "200", "300", "400", "500" };
      var pointsDefaults = new ObservableCollection<string>() { "3", "6", "8", "9", "10", "12", "14", "16", "18", "20", "24", "26", "32", "48", "60" };

      unitList.Add(new ListItem(Itemkey.ScreenPercent, Util.Local.Strings.STR_SCALE_VIEW_PERCENT, Util.Local.Strings.STR_SCALE_VIEW_PERCENT_SHORT, percentDefaults));
      unitList.Add(new ListItem(Itemkey.ScreenFontPoints, Util.Local.Strings.STR_SCALE_VIEW_POINT, Util.Local.Strings.STR_SCALE_VIEW_POINT_SHORT, pointsDefaults));

      return unitList;
    }

    /// <summary>
    /// Reset the dirty flag (e.g. after saving program options when they where edit).
    /// </summary>
    /// <param name="flag"></param>
    internal void SetDirtyFlag(bool flag)
    {
      this.IsDirty = flag;
    }
    #endregion methods
  }
}
