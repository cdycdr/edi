namespace EdiViews.Config.ViewModel
{
  using System;
  using System.Globalization;
  using System.Xml.Serialization;

  using ICSharpCode.AvalonEdit.Highlighting.Themes;
  using Util.Msg;
  using System.Reflection;

  [Serializable]
  [XmlRoot(ElementName = "HighlightingTheme", IsNullable = false)]
  public class HlThemeConfig
  {
    #region fields
    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private HighlightingThemes mStyles;
    private string mPathLocation;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard constructor from human read-able name of highlighting theme
    /// and its file name path in the file system.
    /// </summary>
    /// <param name="hlThemeName">human read-able name of highlighting theme</param>
    /// <param name="fileNamePath">file name path in the file system</param>
    public HlThemeConfig(string hlThemeName, string pathLocation, string fileNamePath)
     :this()
    {
      this.HlThemeName = hlThemeName;
      this.FileNamePath = fileNamePath;
      this.mPathLocation = pathLocation;
    }

    /// <summary>
    /// Standard constructor hidden from public usage
    /// </summary>
    protected HlThemeConfig()
    {
      this.mStyles = null;
      this.mPathLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
      this.FileNamePath = string.Empty;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get/set the name of the highlighting theme (eg.: DeepBlack)
    /// </summary>
    [XmlAttribute(AttributeName = "Name")]
    public string HlThemeName { get; set; }

    /// <summary>
    /// Get/set the location of the current highlighting theme (eg.: C:\DeepBlack.xml)
    /// </summary>
    [XmlAttribute(AttributeName = "Location")]
    public string FileNamePath { get; set; }

    /// <summary>
    /// This property exposes a collection of highlighting themes for different file types
    /// (color and style definitions for keywords in SQL, C# and so forth)
    /// </summary>
    [XmlIgnore]
    public HighlightingThemes HighlightingStyles
    {
      get
      {
        // Lazy load this content when it is needed for the first time ever
        if (this.mStyles == null)
          this.mStyles = ICSharpCode.AvalonEdit.Highlighting.Themes.XML.Read.ReadXML(this.mPathLocation, this.FileNamePath);

        return this.mStyles;
      }
    }
    #endregion properties
  }
}
