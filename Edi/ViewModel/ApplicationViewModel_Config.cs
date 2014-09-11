namespace Edi.ViewModel
{
  using System;
  using System.Windows;
  using EdiViews.Tools.FileExplorer;
  using MsgBox;
  using Settings;
  using Themes;

  public partial class ApplicationViewModel
  {
    /// <summary>
    /// Save application settings when the application is being closed down
    /// </summary>
    public static void SaveConfigOnAppClosed()
    {
      try
      {
        App.CreateAppDataFolder();

        // Save current explorer settings and user profile data
        FileExplorerViewModel.SaveSettings(SettingsManager.Instance, This.FileExplorer);

        // Save program options only if there are un-saved changes that need persistence
        // This can be caused when WPF theme was changed or something else
        // but should normally not occur as often as saving session data
        if (SettingsManager.Instance.SettingData.IsDirty == true)
        {
          SettingsManager.Instance.SaveOptions(App.DirFileAppSettingsData, SettingsManager.Instance.SettingData);
        }

        SettingsManager.Instance.SaveSessionData(App.DirFileAppSessionData, SettingsManager.Instance.SessionData);
      }
      catch (Exception exp)
      {
        MsgBox.Msg.Show(exp, "Unhandled Exception", MsgBoxButtons.OK, MsgBoxImage.Error);
      }
    }

    /// <summary>
    /// Load configuration from persistence on startup of application
    /// </summary>
    public void LoadConfigOnAppStartup()
    {
      // Re/Load program options and user profile session data to control global behaviour of program
      SettingsManager.Instance.LoadOptions(App.DirFileAppSettingsData);
      SettingsManager.Instance.LoadSessionData(App.DirFileAppSessionData);

      SettingsManager.Instance.CheckSettingsOnLoad(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop);

      // Initialize skinning engine with this current skin
      // standard skins defined in class enum
      // plus configured skins with highlighting
      ThemesManager.Instance.SetSelectedTheme(SettingsManager.Instance.SettingData.CurrentTheme);
      this.ResetTheme();                       // Initialize theme in process

      FileExplorerViewModel.LoadSettings(SettingsManager.Instance, This.FileExplorer);
    }
  }
}
