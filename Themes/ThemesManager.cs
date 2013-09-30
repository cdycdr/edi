﻿namespace Themes
{
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Reflection;
  using ICSharpCode.AvalonEdit.Highlighting.Themes;
  using Themes.Definition;

  /// <summary>
  /// This class manages a list of WPF themes (Aero, Metro etc) which
  /// can be combined with TextEditorThemes (True Blue, Deep Black).
  /// 
  /// The class implements a service that can be accessed via Instance property.
  /// The exposed methodes and properties can be used to display a list available
  /// themes, determine the currently selected theme, and set the currently selected
  /// theme.
  /// </summary>
  public class ThemesManager : IParentSelectedTheme
  {
    #region fields
    #region WPF Themes
    #region Aero theme resources
    const string AeroThemeName = "Aero";
    static readonly string[] AeroThemeResources =
    {
      "/FirstFloor.ModernUI;component/Assets/ModernUI.Dark.xaml",
      "/Edi;component/ModernWindowEx.xaml",
      "/Themes;component/Aero/Theme.xaml",
      "/Xceed.Wpf.AvalonDock.Themes.Aero;component/Theme.xaml"
    };
    #endregion Aero theme resources

    #region Expression Dark 2 theme resources
    const string ExpressionDark2ThemeName = "Expression Dark 2";
    static readonly string[] ExpressionDark2Resources =
    {
      "/FirstFloor.ModernUI;component/Assets/ModernUI.Dark.xaml",
      "/Edi;component/ModernWindowEx.xaml",
      
      "/Themes;component/ExpressionDark2/Theme.xaml",
      "/EdiViews;component/Themes/Expressiondark2.xaml",
      "/Xceed.Wpf.AvalonDock.Themes.Expression;component/DarkTheme.xaml"
    };
    #endregion Expression Dark 2 theme resources

    #region Expression Dark theme resources
    const string ExpressionDarkThemeName = "Expression Dark";
    static readonly string[] ExpressionDarkResources = 
    {
      "/Edi;component/Edi.ModernUI.Dark.xaml",
      "/Edi;component/ModernWindowEx.xaml",
      "/Themes;component/ExpressionDark/Theme.xaml",
      "/EdiViews;component/Themes/Expressiondark.xaml",
      "/Xceed.Wpf.AvalonDock.Themes.ExpressionDark;component/Theme.xaml"
    };
    #endregion Expression Dark theme resources

    #region Expression Light 2 theme resources 
    const string ExpressionLight2ThemeName = "Expression Light 2";
    static readonly string[] ExpressionLight2Resources = 
    {
      "/FirstFloor.ModernUI;component/Assets/ModernUI.Dark.xaml",
      "/Edi;component/ModernWindowEx.xaml",
      
      "/Themes;component/ExpressionLight2/Theme.xaml",
      "/EdiViews;component/Themes/ExpressionLight2.xaml",
      "/Xceed.Wpf.AvalonDock.Themes.Expression;component/LightTheme.xaml"
    };
    #endregion Expression Light 2 theme resources

    #region theme resources 
    const string GenericThemeName = "Generic";
    static readonly string[] GenericResources = 
    {
      "/FirstFloor.ModernUI;component/Assets/ModernUI.Dark.xaml",
      "/Edi;component/ModernWindowEx.xaml",
      "/Themes;component/Generic/Theme.xaml",
      "/Xceed.Wpf.AvalonDock.Themes.Aero;component/Theme.xaml"
    };
    #endregion theme resources

    #region theme resources 
    const string MetroThemeName = "Metro";
    static readonly string[] MetroResources = 
    {
      "/FirstFloor.ModernUI;component/Assets/ModernUI.Dark.xaml",
      "/Edi;component/ModernWindowEx.xaml",
      "/Themes;component/Metro/Theme.xaml",
      "/Xceed.Wpf.AvalonDock.Themes.Metro;component/Theme.xaml"
    };
    #endregion theme resources

    #region VS2010 theme resources 
    const string VS2010 = "VS 2010";
    static readonly string[] VS2010Resources = 
    {
      "/FirstFloor.ModernUI;component/Assets/ModernUI.Dark.xaml",
      "/Edi;component/ModernWindowEx.xaml",
      "/Themes;component/VS2010/Theme.xaml",
      "/Xceed.Wpf.AvalonDock.Themes.VS2010;component/Theme.xaml"
    };
    #endregion VS2010 theme resources
    #endregion WPF Themes

    #region Text Editor Themes
    const string EditorThemeBrightStandard = "Bright Standard";
    const string EditorThemeBrightStandardLocation = @"AvalonEdit\HighLighting_Themes\BrightStandard.xshd";

    const string EditorThemeTrueBlue = "True Blue";
    const string EditorThemeTrueBlueLocation = @"AvalonEdit\HighLighting_Themes\TrueBlue.xshd";

    const string EditorThemeDeepBlack = "Deep Black";
    const string EditorThemeDeepBlackLocation = @"AvalonEdit\HighLighting_Themes\DeepBlack.xshd";
    #endregion Text Editor Themes

    public const string DefaultThemeName = AeroThemeName;

    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private SortedDictionary<string, EditorThemeBase> mTextEditorThemes = null;
    private ObservableCollection<EditorThemeBase> mListOfAllThemes = null;
    private string mSelectedThemeName = string.Empty;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public ThemesManager ()
	  {
      this.mSelectedThemeName = ThemesManager.DefaultThemeName;
	  }
    #endregion constructor

    #region properties
    /// <summary>
    /// Gets the default <seealso cref="ThemesManager"/> instance.
    /// The default <seealso cref="ThemesManager"/> comes with the built-in WPF andHighlighting themes.
    /// </summary>
    public static ThemesManager Instance
    {
      get
      {
        return DefaultThemesManager.Instance;
      }
    }

    /// <summary>
    /// Get the name of the currently seelcted theme.
    /// </summary>
    public string SelectedThemeName
    {
      get
      {
        return this.mSelectedThemeName;
      }
    }

    /// <summary>
    /// Get the object that has links to all resources for the currently selected WPF theme.
    /// </summary>
    public EditorThemeBase SelectedTheme
    {
      get
      {
        if (this.mTextEditorThemes == null || this.mListOfAllThemes == null)
          this.BuildThemeCollections();

        EditorThemeBase theme;
        this.mTextEditorThemes.TryGetValue(this.mSelectedThemeName, out theme);

        // Fall back to default if all else fails
        if (theme == null)
        {
          this.mTextEditorThemes.TryGetValue(ThemesManager.DefaultThemeName, out theme);
          this.mSelectedThemeName = theme.HlThemeName;
        }

        return theme;
      }
    }

    /// <summary>
    /// Get a list of all available themes (This property can typically be used to bind
    /// menuitems or other resources to let the user select a theme in the user interface).
    /// </summary>
    public ObservableCollection<EditorThemeBase> ListAllThemes
    {
      get
      {
        if (this.mTextEditorThemes == null || this.mListOfAllThemes == null)
          this.BuildThemeCollections();

        return this.mListOfAllThemes;
      }      
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Change the WPF/EditorHighlightingTheme to the <paramref name="themeName"/> theme.
    /// </summary>
    /// <param name="themeName"></param>
    /// <returns>True if new theme is succesfully selected (was available), otherwise false</returns>
    public bool SetSelectedTheme(string themeName)
    {
      if (this.mTextEditorThemes == null || this.mListOfAllThemes == null)
        this.BuildThemeCollections();

      EditorThemeBase theme;
      this.mTextEditorThemes.TryGetValue(themeName, out theme);

      // Fall back to default if all else fails
      if (theme == null)
        return false;

      this.mSelectedThemeName = themeName;

      return true;
    }

    /// <summary>
    /// Get a text editor highlighting theme associated with the given WPF Theme Name.
    /// </summary>
    /// <param name="themeName"></param>
    /// <returns></returns>
    public HighlightingThemes GetTextEditorHighlighting(string themeName)
    {
      // Is this WPF theme configured with a highlighting theme???
      EditorThemeBase cfg = null;

      this.mTextEditorThemes.TryGetValue(themeName, out cfg);

      if (cfg != null)
        return cfg.HighlightingStyles;

      return null;
    }

    /// <summary>
    /// Build sorted dictionary and observable collection for WPF themes.
    /// </summary>
    private void BuildThemeCollections()
    {
      this.mTextEditorThemes = this.BuildThemeDictionary();
      this.mListOfAllThemes = new ObservableCollection<EditorThemeBase>();

      foreach (KeyValuePair<string, EditorThemeBase> t in this.mTextEditorThemes)
      {
        this.mListOfAllThemes.Add(t.Value);
      }
    }

    /// <summary>
    /// Build a sorted structure of all default themes and their resources.
    /// </summary>
    /// <returns></returns>
    private SortedDictionary<string, EditorThemeBase> BuildThemeDictionary()
    {
      SortedDictionary<string, EditorThemeBase> ret = new SortedDictionary<string, EditorThemeBase>();

      EditorThemeBase t = null;
      string themeName = null;
      List<string> wpfTheme = null;

      try
      {
        string appLocation = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

        //Aero themes
        themeName = AeroThemeName;
        wpfTheme = new List<string>(AeroThemeResources);

        t = new EditorThemeBase(this, wpfTheme, themeName, null, null, null);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeBrightStandard,
                                appLocation, EditorThemeBrightStandardLocation);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeTrueBlue,
                                appLocation, EditorThemeTrueBlueLocation);
        ret.Add(t.HlThemeName, t);

        // ExpressionDark2 Themes
        themeName = ExpressionDark2ThemeName;
        wpfTheme = new List<string>(ExpressionDark2Resources);

        t = new EditorThemeBase(this, wpfTheme, themeName, null, null, null);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeDeepBlack,
                                appLocation, EditorThemeDeepBlackLocation);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeTrueBlue,
                                appLocation, EditorThemeTrueBlueLocation);
        ret.Add(t.HlThemeName, t);

        // ExpressionDark Theme
        themeName = ExpressionDarkThemeName;
        wpfTheme = new List<string>(ExpressionDarkResources);

        t = new EditorThemeBase(this, wpfTheme, themeName, null, null, null);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeDeepBlack,
                                appLocation, EditorThemeDeepBlackLocation);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeTrueBlue,
                                appLocation, EditorThemeTrueBlueLocation);
        ret.Add(t.HlThemeName, t);

        // Expression Light
        themeName = ExpressionLight2ThemeName;
        wpfTheme = new List<string>(ExpressionLight2Resources);

        t = new EditorThemeBase(this, wpfTheme, themeName, null, null, null);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeBrightStandard,
                                appLocation, EditorThemeBrightStandardLocation);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeTrueBlue,
                                appLocation, EditorThemeTrueBlueLocation);
        ret.Add(t.HlThemeName, t);

        // Generic Theme
        themeName = GenericThemeName;
        wpfTheme = new List<string>(GenericResources);

        t = new EditorThemeBase(this, wpfTheme, themeName, null, null, null);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeBrightStandard,
                                appLocation, EditorThemeBrightStandardLocation);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeTrueBlue,
                                appLocation, EditorThemeTrueBlueLocation);
        ret.Add(t.HlThemeName, t);

        // Metro Theme
        themeName = MetroThemeName;
        wpfTheme = new List<string>(MetroResources);

        t = new EditorThemeBase(this, wpfTheme, themeName, null, null, null);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeBrightStandard,
                                appLocation, EditorThemeBrightStandardLocation);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeTrueBlue,
                                appLocation, EditorThemeTrueBlueLocation);
        ret.Add(t.HlThemeName, t);

        // VS 2010 Theme
        themeName = VS2010;
        wpfTheme = new List<string>(VS2010Resources);

        t = new EditorThemeBase(this, wpfTheme, themeName, null, null, null);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeBrightStandard,
                                appLocation, EditorThemeBrightStandardLocation);
        ret.Add(t.HlThemeName, t);

        t = new EditorThemeBase(this, wpfTheme, themeName, EditorThemeTrueBlue,
                                appLocation, EditorThemeTrueBlueLocation);
        ret.Add(t.HlThemeName, t);
      }
      catch (System.Exception exp)
      {
        string msg = string.Format("Error registering application theme '{0}' -> '{1}'", 
                                    themeName == null ? "(null)" : themeName,
                                    t == null ? "(null)" : t.HlThemeName);

        // Log an error message and let the system boot up with default theme instead of re-throwing this
        logger.Fatal(new System.Exception(msg, exp));
      }

      return ret;
    }
    #endregion methods

    #region internal classes
    /// <summary>
    /// Internal implementation class of the static ThemesManager.Instance property.
    /// </summary>
    internal sealed class DefaultThemesManager : ThemesManager
    {
      public new static readonly DefaultThemesManager Instance = new DefaultThemesManager();
    }
    #endregion internal classes
  }
}
