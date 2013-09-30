namespace Edi.ViewModel
{
  using System;
  using EdiViews.Config.ViewModel;
  using MsgBox;
  using Themes;

  public partial class Workspace
  {
    #region fields
    private ConfigViewModel _config = null;
    #endregion fields

    /// <summary>
    /// Get/set program settings for entire application
    /// </summary>
    public ConfigViewModel Config
    {
      get
      {
        if (_config == null)
          _config = new ConfigViewModel();

        return _config;
      }

      internal set
      {
        if (_config != value)
        {
          _config = value;
          this.NotifyPropertyChanged(() => this.Config);
        }
      }
    }

    /// <summary>
    /// Save application settings when the application is being closed down
    /// </summary>
    public void SaveConfigOnAppClosed()
    {
      try
      {
        App.CreateAppDataFolder();

        // Save/initialize program options that determine global program behaviour
        ConfigViewModel.SaveOptions(App.DirFileAppSessionData, this.Config);
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
      ConfigViewModel retOpts = null;

      try
      {
        // Re/Load program options to control global behaviour of program
        if ((retOpts = ConfigViewModel.LoadOptions(App.DirFileAppSessionData)) == null)
          retOpts = new ConfigViewModel();
      }
      catch
      {
      }
      finally
      {
        if (retOpts == null)
          retOpts = new ConfigViewModel();
      }

      this.Config = retOpts;

      // Initialize skinning engine with this current skin
      // standard skins defined in class enum
      // plus configured cosumt skins with highlighting
      
      ThemesManager.Instance.SetSelectedTheme(this.Config.CurrentTheme);
      this.ResetTheme();                       // Initialize theme in process
    }
  }
}
