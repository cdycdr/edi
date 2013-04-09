namespace EdiViews.Config.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Xml.Serialization;
  using System.Globalization;

  public class HlThemeKey
  {
    #region Fields
    private EdiThemesViewModel.WPFTheme mAppTheme;
    private string mHlThemeName;
    #endregion Fields

    #region Constructor
    /// <summary>
    /// Constructor
    /// </summary>
    public HlThemeKey()
    {
      this.mAppTheme = EdiThemesViewModel.DefaultWPFTheme;
      this.mHlThemeName = string.Empty;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="enumeration"></param>
    /// <param name="hlThemeName"></param>
    public HlThemeKey(EdiThemesViewModel.WPFTheme enumeration,
                      string hlThemeName = null)
    {
        this.mAppTheme = enumeration;
        this.mHlThemeName = hlThemeName;
    }

    /// <summary>
    /// Copy constructor
    /// </summary>
    /// <param name="copyThis"></param>
    public HlThemeKey(HlThemeKey copyThis)
    {
      if (copyThis != null)
        return;

      this.mAppTheme = copyThis.mAppTheme;
      this.mHlThemeName = copyThis.mHlThemeName;
    }
    #endregion Constructor

    #region Properties
    /// <summary>
    /// Get/set custom theme name
    /// </summary>
    public string HlThemeName
    {
      get
      {
        return (this.mHlThemeName == null ? EdiThemesViewModel.MapEnumToName(this.AppTheme) : this.mHlThemeName);
      }

      set
      {
        this.mHlThemeName = value;
      }
    }

    /// <summary>
    /// Get/set WPF theme
    /// </summary>
    public EdiThemesViewModel.WPFTheme AppTheme
    {
      get { return this.mAppTheme; }
      set { this.mAppTheme = value; }
    }
    #endregion Properties

    #region Methods
    /// <summary>
    /// Serves as a hash function for a particular type.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
      return this.mAppTheme.GetHashCode() |
             (this.mHlThemeName == null ? 0 : this.mHlThemeName.GetHashCode());
    }

    /// <summary>
    /// Determines whether the specified object is equal to the current object.
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        var supplied = obj as HlThemeKey;
        if (supplied == null)
          return false;

        if (supplied.mAppTheme != this.mAppTheme)
          return false;

        if (supplied.HlThemeName != this.HlThemeName)
          return false;

        return true;
    }
    #endregion Methods
  }
}
