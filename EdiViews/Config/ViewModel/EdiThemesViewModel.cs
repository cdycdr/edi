namespace EdiViews.Config.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Collections.ObjectModel;
  using System.Windows;
  using System.Reflection;
  using System.Globalization;

  /// <summary>
  /// Theme (application skin) viewModel for Exclusive Test enumeration.
  /// That is, each theme in the application is associated with one instance
  /// of the <seealso cref="ThemeVM"/> class. But only one object of that
  /// class has <seealso cref="ThemeVM.IsChecked"/> == true at a time.
  /// </summary>
  public class EdiThemeViewModel
  {
    #region fields
    private EdiThemesViewModel mParent = null;
    private string mText = string.Empty;
    #endregion fields

    #region constructor
    /// <summary>
    /// Constructor from parent reference to <seealso cref="ThemesVM"/>
    /// collection and <see cref="ThemesVM.EnTheme"/> enumeration entry.
    /// </summary>
    /// <param name="parentObject"></param>
    /// <param name="displayText"></param>
    /// <param name="t"></param>
    public EdiThemeViewModel(EdiThemesViewModel parentObject, HlThemeKey t, string displayText)
    {
      this.mParent = parentObject;
      this.ThemeID = t;
      this.mText = displayText;
    }

    private EdiThemeViewModel()
    {
    }
    #endregion constructor

    #region Properties
    /// <summary>
    /// Get/set enumeration type for this theme
    /// </summary>
    public HlThemeKey ThemeID { get; set; }

    /// <summary>
    /// Get human-readable name of this theme
    /// </summary>
    public string Text
    {
      get
      {
        return this.mText;
      }
    }

    public bool IsChecked
    {
      get
      {
        if (this.mParent != null)
	      {
		      if (this.mParent.CurrentTheme != null)
	        {
		        return this.mParent.CurrentTheme.Equals(this.ThemeID);
	        }
	      }

        return false; 
      }
    }
    #endregion Properties
  }

  /// <summary>
  /// This class is an extension of the <seealso cref="Themes.ThemesVM"/> class.
  /// It enables WPF themes (themed WPF controls - buttons etc) plus themed highlighting
  /// patterns applied in the editor control (AvalonEdit). Hence one WPF theme may be
  /// applicable multiple times (with different highlighting themes).
  /// </summary>
  public class EdiThemesViewModel
  {
    #region fields
    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    /// <summary>
    /// Default WPF theme.
    /// </summary>
    public static WPFTheme DefaultWPFTheme = WPFTheme.Aero;

    /// <summary>
    /// Array of theme names for each application theme
    /// 
    /// <seealso cref="EnTheme"/>
    /// </summary>
    private static readonly string[] ThemeNames =
    {
      "Aero",
      "VS2010",
      "Generic",
      "Expression Dark", // WPFTheme.ExpressionDark
      "Metro",
      "Expression Dark 2",
      "Expression Light 2"
    };

    private HlThemeKey mCurrentTheme;
    #endregion fields

    #region constructor
    /// <summary>
    /// Default constructor for collection of themes (application skins).
    /// </summary>
    public EdiThemesViewModel()
    {
      this.Themes = EdiThemesViewModel.ListAllThemes(this);

      // Set default theme
      this.mCurrentTheme = new HlThemeKey(EdiThemesViewModel.DefaultWPFTheme);
    }

    /// <summary>
    /// Default constructor for collection of themes (application skins)
    /// from <seealso cref="ThemesVM.EnTheme"/> enumeration entry as theme
    /// to initialize to.
    /// </summary>
    /// <param name="thisCurrentTheme"></param>
    public EdiThemesViewModel(HlThemeKey thisCurrentTheme,
                              IEnumerator<KeyValuePair<HlThemeKey, HlThemeConfig>> dic = null)
    {
      // Create a list of all build-in enumeration based themes with highlighting themes
      this.Themes = EdiThemesViewModel.ListAllThemes(this, dic);

      try 
	    {	        
        this.mCurrentTheme = new HlThemeKey(thisCurrentTheme);    // Construct with this theme as current theme
	    }
	    catch (Exception exp)
	    {
        logger.Error("Error resetting to highlighting theme from last user session.", exp);
	    }
    }
    #endregion constructor

    #region enums
    /// <summary>
    /// Enumeration for supported application themes
    /// 
    /// These enumerations AND their indexes
    /// must be consistent with the <seealso cref="ThemeNames"/> array.
    /// </summary>
    public enum WPFTheme
    {
      /// <summary>
      /// VS2010 theme selector
      /// </summary>
      Aero = 0,

      /// <summary>
      /// VS2010 theme selector
      /// </summary>
      VS2010 = 1,

      /// <summary>
      /// Generic theme selector
      /// </summary>
      Generic = 2,

      /// <summary>
      /// ExpressionDark theme selector
      /// </summary>
      ExpressionDark = 3,

      /// <summary>
      /// Metro theme selector
      /// </summary>
      Metro = 4,

      /// <summary>
      /// ExpressionDark2 theme selector
      /// </summary>
      ExpressionDark2 = 5,

      /// <summary>
      /// ExpressionLight theme selector
      /// </summary>
      ExpressionLight2 = 6
    }
    #endregion enums

    #region properties
    /// <summary>
    /// Get/set WPF theme for the complete Application
    /// </summary>
    public HlThemeKey CurrentTheme
    {
      get
      {
        return this.mCurrentTheme;
      }

      set
      {
        this.mCurrentTheme = value;
      }
    }

    /// <summary>
    /// Instances of themes collection managed in this object.
    /// Menus or other GUI controls can bind to this collection
    /// to detect/change the currently selected theme.
    /// </summary>
    public ObservableCollection<EdiThemeViewModel> Themes
    {
      get;
      private set;
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Map the name of a theme to its enumeration type
    /// </summary>
    /// <param name="sThemeName"></param>
    /// <returns></returns>
    public static WPFTheme MapNameToEnum(string sThemeName)
    {
      if (sThemeName != null)                 // Select theme by name if one was given
      {                                      // Check if requested theme is available

        foreach (WPFTheme t in Enum.GetValues(typeof(WPFTheme)))
        {
          if (t.ToString().ToLower() == sThemeName.ToLower())
          {
            return t;
          }
        }
      }

      throw new NotImplementedException((sThemeName == null ? "(null)" : sThemeName));
    }

    public static string MapEnumToName(WPFTheme wpfTheme)
    {
      return EdiThemesViewModel.ThemeNames[(int)wpfTheme];
    }

    /// <summary>
    /// Load a resource dictionary from a theming component
    /// </summary>
    /// <param name="theme"></param>
    /// <param name="componentName"></param>
    /// <returns></returns>
    public static ResourceDictionary GetThemeResourceDictionary(string theme, string componentName = "Edi.exe")
    {
      if (theme != null)
      {
        Assembly assembly = Assembly.LoadFrom(componentName);
        string packUri = string.Format(CultureInfo.CurrentCulture, @"/Edi;component/Themes/{0}/Theme.xaml", theme);

        return Application.LoadComponent(new Uri(packUri, UriKind.Relative)) as ResourceDictionary;
      }

      return null;
    }

    /// <summary>
    /// List all themes available in this component
    /// </summary>
    /// <returns></returns>
    private static ObservableCollection<EdiThemeViewModel> ListAllThemes(EdiThemesViewModel themes,
                                                                         IEnumerator<KeyValuePair<HlThemeKey, HlThemeConfig>> dic = null)
    {
      // Sort themes by their name before constructing collection of themes
      System.Collections.Generic.SortedDictionary<string, EdiThemeViewModel> s = new SortedDictionary<string, EdiThemeViewModel>();

      foreach (object t in Enum.GetValues(typeof(EdiThemesViewModel.WPFTheme)))
      {
        s.Add(EdiThemesViewModel.ThemeNames[(int)t],
              new EdiThemeViewModel(themes, new HlThemeKey((EdiThemesViewModel.WPFTheme)t), EdiThemesViewModel.ThemeNames[(int)t]));
      }

      try
      {
        // Add costom themes containing additional highlighting themes
        if (dic != null)
        {
          while (dic.MoveNext() == true)
          {
            s.Add(dic.Current.Key.HlThemeName, new EdiThemeViewModel(themes, dic.Current.Key, dic.Current.Key.HlThemeName));
          }
        }
      }
      catch (Exception exp)
      {
        logger.Error("Error registering custom highlighting theme.", exp);
      }

      ObservableCollection<EdiThemeViewModel> allThemes = new ObservableCollection<EdiThemeViewModel>();
      foreach (KeyValuePair<string, EdiThemeViewModel> t in s)
      {
        allThemes.Add(t.Value);
      }

      return allThemes;
    }
    #endregion methods
  }
}
