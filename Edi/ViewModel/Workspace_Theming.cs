namespace Edi.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Reflection;
  using System.Windows;

  using Edi.AvalonEdit.Highlighting;
  using EdiViews.Config.ViewModel;
  using ICSharpCode.AvalonEdit.Highlighting;
  using MsgBox;
  using ICSharpCode.AvalonEdit.Highlighting.Themes;
  using System.Windows.Media;

  partial class Workspace
  {
    #region Themes
    /// <summary>
    /// Expose the application theme currently applicable
    /// (see Also themining in <seealso cref="ConfigViewModel"/> )
    /// </summary>
    public EdiThemesViewModel Skins { get; set; }
    #endregion Themes

    /// <summary>
    /// Change WPF theme to theme supplied in <paramref name="themeToSwitchTo"/>
    /// 
    /// This method can be called when the theme is to be reseted by all means
    /// (eg.: when powering application up).
    /// 
    /// !!! Use the <seealso cref="CurrentTheme"/> property to change !!!
    /// !!! the theme when App is running                             !!!
    /// </summary>
    /// <param name="themeToSwitchTo"></param>
    public void ResetTheme(object sender, EventArgs args)
    {
      // Reset customized resources (if there are any from last change) and
      // enforce reload of original values from resource dictionary
      if (HighlightingManager.Instance.BackupDynResources != null)
      {
        try
        {
          foreach(string t in HighlightingManager.Instance.BackupDynResources)
          {
            Application.Current.Resources[t] = null;
          }
        }
        catch
        {
        }
        finally
        {
          HighlightingManager.Instance.BackupDynResources = null;
        }
      }

      HlThemeKey themeToSwitchTo = new HlThemeKey(EdiThemesViewModel.DefaultWPFTheme);

      if (this.Config != null)
      {
        themeToSwitchTo = this.Config.CurrentTheme;

        this.SwitchToSelectedTheme(null, themeToSwitchTo);
      }

      // Backup highlighting names (if any) and restore highlighting associations after reloading highlighting definitions
      List<string> HlNames = new List<string>();

      foreach (EdiViewModel f in this.Documents)
      {
        if (f != null)
        {
          if (f.HighlightingDefinition != null)
            HlNames.Add(f.HighlightingDefinition.Name);
          else
            HlNames.Add(null);
        }
      }

      // Is the current theme configured with a highlighting theme???
      HighlightingThemes hlThemes = this.Config.FindHighlightingTheme(themeToSwitchTo);

      // Re-load all highlighting patterns and re-apply highlightings
      HighlightingExtension.RegisterCustomHighlightingPatterns(hlThemes);

      //Re-apply highlightings after resetting highlighting manager

      List<EdiViewModel> l = this.Documents;
      for (int i = 0; i < l.Count; i++)
      {
        if (l[i] != null)
        {
          IHighlightingDefinition hdef = HighlightingManager.Instance.GetDefinition(HlNames[i]);

            if (hdef != null)
              l[i].HighlightingDefinition = hdef;
        }
      }

      List<string> backupDynResources = new List<string>();

      // Apply global styles to theming elements (dynamic resources in resource dictionary) of editor control
      if (HighlightingManager.Instance.HlThemes != null)
      {
        foreach(WidgetStyle w in HighlightingManager.Instance.HlThemes.GlobalStyles)
          ApplyWidgetStyle(w, backupDynResources);
      }

      if (backupDynResources.Count > 0)
        HighlightingManager.Instance.BackupDynResources = backupDynResources;
    }

    /// <summary>
    /// Applies a widget style to a dynamic resource in the WPF Resource Dictionary.
    /// The intention is to be more flexible and allow users to configure other editor
    /// theme colors than thoses that are pre-defined.
    /// </summary>
    /// <param name="w"></param>
    private void ApplyWidgetStyle(WidgetStyle w,
                                  List<string> backupDynResources)
    {
      if (w == null)
        return;

      switch (w.Name)
      {
        case "DefaultStyle":
          ApplyToDynamicResource("EditorBackground", w.bgColor, backupDynResources);
          ApplyToDynamicResource("EditorForeground", w.fgColor, backupDynResources);
          break;

        case "CurrentLineBackground":
          ApplyToDynamicResource("EditorCurrentLineBackgroundColor", w.bgColor, backupDynResources);
          break;

        case "LineNumbersForeground":
          ApplyToDynamicResource("EditorLineNumbersForeground", w.fgColor, backupDynResources);
          break;

        case "Selection":
          ApplyToDynamicResource("EditorSelectionBrush", w.bgColor, backupDynResources);
          ApplyToDynamicResource("EditorSelectionBorder", w.borderColor, backupDynResources);
          ApplyToDynamicResource("EditorSelectionForeground", w.fgColor, backupDynResources);
          break;

        case "Hyperlink":
          ApplyToDynamicResource("LinkTextBackgroundBrush", w.bgColor, backupDynResources);
          ApplyToDynamicResource("LinkTextForegroundBrush", w.fgColor, backupDynResources);
          break;

        case "NonPrintableCharacter":
          ApplyToDynamicResource("NonPrintableCharacterBrush", w.fgColor, backupDynResources);
          break;

        default:
          logger.WarnFormat("WidgetStyle named '{0}' is not supported.", w.Name);
          break;
      }
    }
    
    /// <summary>
    /// Re-define an existing <seealso cref="SolidColorBrush"/> and backup the originial color
    /// as it was before the application of the custom coloring.
    /// </summary>
    /// <param name="ResourceName"></param>
    /// <param name="NewColor"></param>
    /// <param name="backupDynResources"></param>
    private void ApplyToDynamicResource(string ResourceName,
                                        SolidColorBrush NewColor,
                                        List<string> backupDynResources)
    {
      if (Application.Current.Resources[ResourceName] != null && NewColor != null)
      {
        // Re-coloring works with SolidColorBrushs linked as DynamicResource
        if (Application.Current.Resources[ResourceName] is SolidColorBrush)
        {
          SolidColorBrush oldBrush = Application.Current.Resources[ResourceName] as SolidColorBrush;
          backupDynResources.Add(ResourceName);

          Application.Current.Resources[ResourceName] = NewColor.Clone();
        }
      }
    }

    /// <summary>
    /// Attempt to switch to the theme as stated in <paramref name="sParameter"/>.
    /// The given name must map into the <seealso cref="Themes.ThemesVM.EnTheme"/> enumeration.
    /// </summary>
    /// <param name="sParameter"></param>
    /// <param name="thisTheme"></param>
    private bool SwitchToSelectedTheme(string sParameter = null, HlThemeKey thisTheme = null)
    {
      const string themesModul = "Themes.dll";
      
      try
      {
        if (sParameter != null)
          thisTheme = new HlThemeKey(EdiThemesViewModel.MapNameToEnum(sParameter)); // Select theme by name if one was given

        this.Skins.CurrentTheme = (thisTheme == null ? new HlThemeKey(EdiThemesViewModel.WPFTheme.Aero) : thisTheme);

        string[] Uris = null;

        switch (thisTheme.AppTheme)
        {
          case EdiThemesViewModel.WPFTheme.Aero:
            Uris = new string[2];

            Uris[0] = "/Themes;component/Aero/Theme.xaml";
            Uris[1] = "/AvalonDock.Themes.Aero;component/Theme.xaml";
            break;

          case EdiThemesViewModel.WPFTheme.ExpressionDark:
            Uris = new string[3];

            Uris[0] = "/Themes;component/ExpressionDark/Theme.xaml";
            Uris[1] = "/EdiViews;component/Themes/Expressiondark.xaml";
            Uris[2] = "/AvalonDock.Themes.ExpressionDark;component/Theme.xaml";
            break;

          case EdiThemesViewModel.WPFTheme.VS2010:
            Uris = new string[2];

            Uris[0] = "/Themes;component/VS2010/Theme.xaml";
            Uris[1] = "/AvalonDock.Themes.VS2010;component/Theme.xaml";
            break;

          case EdiThemesViewModel.WPFTheme.Generic:
            Uris = new string[1];

            Uris[0] = "/Themes;component/Generic/Theme.xaml";
            break;

          case EdiThemesViewModel.WPFTheme.Metro:
            Uris = new string[2];

            Uris[0] = "/Themes;component/Metro/Theme.xaml";
            Uris[1] = "/AvalonDock.Themes.Metro;component/Theme.xaml";
            break;

          case EdiThemesViewModel.WPFTheme.ExpressionDark2:
            Uris = new string[3];

            Uris[0] = "/Themes;component/ExpressionDark2/Theme.xaml";
            Uris[1] = "/EdiViews;component/Themes/Expressiondark2.xaml";
            Uris[2] = "/AvalonDock.Themes.Expression;component/DarkTheme.xaml";
            break;

          case EdiThemesViewModel.WPFTheme.ExpressionLight2:
            Uris = new string[3];

            Uris[0] = "/Themes;component/ExpressionLight2/Theme.xaml";
            Uris[1] = "/EdiViews;component/Themes/ExpressionLight2.xaml";
            Uris[2] = "/AvalonDock.Themes.Expression;component/LightTheme.xaml";
            break;

          default:
            break;
        }

        if (Uris != null)
        {
          Application.Current.Resources.MergedDictionaries.Clear();

          string ThemesPathFileName = Assembly.GetEntryAssembly().Location;

          ThemesPathFileName = System.IO.Path.GetDirectoryName(ThemesPathFileName);
          ThemesPathFileName = System.IO.Path.Combine(ThemesPathFileName, themesModul);
          Assembly assembly = Assembly.LoadFrom(ThemesPathFileName);

          if (System.IO.File.Exists(ThemesPathFileName) == false)
          {
            MsgBox.Msg.Box.Show(string.Format(CultureInfo.CurrentCulture, "Cannot find Path to: '{0}'\n\n" +
                              "Please make sure this module is accesible.", themesModul), "Error",
                              MsgBoxButtons.OK, MsgBoxImage.Error);

            return false;
          }

          for (int i = 0; i < Uris.Length; i++)
          {
            try
            {
              Uri Res = new Uri(Uris[i], UriKind.Relative);

              ResourceDictionary dictionary = Application.LoadComponent(Res) as ResourceDictionary;

              if (dictionary != null)
                Application.Current.Resources.MergedDictionaries.Add(dictionary);
            }
            catch (Exception Exp)
            {
              MsgBox.Msg.Box.Show(Exp, string.Format(CultureInfo.CurrentCulture, "'{0}'", Uris[i]), MsgBoxButtons.OK, MsgBoxImage.Error);
            }
          }
        }
      }
      catch (Exception exp)
      {
        MsgBox.Msg.Box.Show(exp, "Error", MsgBoxButtons.OK, MsgBoxImage.Error);

        return false;
      }

      return true;
    }
  }
}
