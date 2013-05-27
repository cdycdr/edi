using System;
using System.Configuration;
namespace MiniUML.Framework
{
  using MsgBox;

  public static class SettingsManager
  {
    public static ApplicationSettingsBase Settings;

    public static void SaveSettings()
    {
      try
      {
        Settings.Save();
      }
      catch (Exception ex)
      {
        // "The MiniUML preferences were not saved properly."
        MsgBox.Msg.Show(ex, "Could not save preferences.", MsgBoxButtons.OK, MsgBoxImage.Error);
      }
    }
  }
}
