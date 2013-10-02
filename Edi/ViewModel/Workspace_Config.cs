namespace Edi.ViewModel
{
  using System;
  using System.Windows;
  using EdiViews.Config.ViewModel;
  using MsgBox;
  using Settings;
  using Themes;

  public partial class Workspace
  {
    #region fields
    private ConfigViewModel _config = null;
    #endregion fields

    /// <summary>
    /// Save application settings when the application is being closed down
    /// </summary>
    public void SaveConfigOnAppClosed()
    {
      try
      {
        App.CreateAppDataFolder();

        SettingsManager.Instance.SaveOptions(App.DirFileAppSettingsData, SettingsManager.Instance.SettingData);
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
      // plus configured cosumt skins with highlighting
      
      ThemesManager.Instance.SetSelectedTheme(SettingsManager.Instance.SettingData.CurrentTheme);
      this.ResetTheme();                       // Initialize theme in process
    }
  }
}
