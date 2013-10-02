namespace Settings.ProgramSettings
{
  using System;
  using System.Xml.Serialization;
  using ICSharpCode.AvalonEdit.Highlighting.Themes;
  using SimpleControls.MRU.ViewModel;
  using Themes;

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
    public const int TextEditorZoomInFontSize = 1;
    public const int TextEditorZoomInPercent = 0;

    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private string mCurrentTheme;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class Constructor
    /// </summary>
    public Options()
    {
      this.mCurrentTheme = ThemesManager.DefaultThemeName;

      this.DocumentZoomUnit = TextEditorZoomInPercent;     // Zoom View in Percent
      this.DocumentZoomView = 150;                        // Font Size 12 is 100 %
      this.ReloadOpenFilesOnAppStart = true;
      this.RunSingleInstance = true;
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

    [XmlElement(ElementName = "RunSingleInstance")]
    public bool RunSingleInstance { get; set; }

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
          this.mCurrentTheme = value;
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
    #endregion methods
  }
}
