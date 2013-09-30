namespace Edi
{
  using System.Collections.Generic;
  using Edi.View;
  using EdiViews.Config.ViewModel;
  using Themes;

  /// <summary>
  /// This class manages global settings such as a
  /// 
  /// 1> MainMenu control,
  /// </summary>
  public static class MainWindowViewManager
  {
    #region fields
    private static MainMenu mMainMenu = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Staic class constructor
    /// </summary>
    static MainWindowViewManager()
    {
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get the currently available main menu to be displayed in the main menu.
    /// </summary>
    public static MainMenu MainWindowMenu
    {
      get
      {
        if (MainWindowViewManager.mMainMenu == null)
          MainWindowViewManager.mMainMenu = new MainMenu();

        return MainWindowViewManager.mMainMenu;
      }
    }
    #endregion properties
  }
}
