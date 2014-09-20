namespace EdiViews.ViewModel
{
	using System;
	using System.Windows;
	using Edi.Core.Models;
	using EdiTools.ViewModels.FileExplorer;
	using MsgBox;
	using Settings;
	using Settings.ProgramSettings;
	using Settings.UserProfile;
	using Themes;

	public partial class ApplicationViewModel
	{
		/// <summary>
		/// Save application settings when the application is being closed down
		/// </summary>
		public void SaveConfigOnAppClosed()
		{
			try
			{
				this.mAppCore.CreateAppDataFolder();

				// Save current explorer settings and user profile data
				FileExplorerViewModel.SaveSettings(SettingsManager.Instance, this.FileExplorer);

				// Save program options only if there are un-saved changes that need persistence
				// This can be caused when WPF theme was changed or something else
				// but should normally not occur as often as saving session data
				if (SettingsManager.Instance.SettingData.IsDirty == true)
				{
					SettingsManager.Instance.SaveOptions(this.mAppCore.DirFileAppSettingsData, SettingsManager.Instance.SettingData);
				}

				SettingsManager.Instance.SaveSessionData(this.mAppCore.DirFileAppSessionData, SettingsManager.Instance.SessionData);
			}
			catch (Exception exp)
			{
				MsgBox.Msg.Show(exp, "Unhandled Exception", MsgBoxButtons.OK, MsgBoxImage.Error);
			}
		}

		/// <summary>
		/// Load configuration from persistence on startup of application
		/// </summary>
		public void LoadConfigOnAppStartup(Options programSettings)
		{
			// Re/Load program options and user profile session data to control global behaviour of program
			SettingsManager.Instance.LoadOptions(this.mAppCore.DirFileAppSettingsData, programSettings);
			SettingsManager.Instance.LoadSessionData(this.mAppCore.DirFileAppSessionData);

			SettingsManager.Instance.CheckSettingsOnLoad(SystemParameters.VirtualScreenLeft, SystemParameters.VirtualScreenTop);

			// Initialize skinning engine with this current skin
			// standard skins defined in class enum
			// plus configured skins with highlighting
			ThemesManager.Instance.SetSelectedTheme(SettingsManager.Instance.SettingData.CurrentTheme);
			this.ResetTheme();                       // Initialize theme in process

			FileExplorerViewModel.LoadSettings(SettingsManager.Instance, this.FileExplorer);
		}

		/// <summary>
		/// Save session data on closing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			try
			{
				if (this.Exit_CheckConditions(sender) == true)      // Close all open files and check whether application is ready to close
				{
					this.OnRequestClose();                          // (other than exception and error handling)

					e.Cancel = false;
					//if (wsVM != null)
					//  wsVM.SaveConfigOnAppClosed(); // Save application layout
				}
				else
					e.Cancel = this.ShutDownInProgress_Cancel = true;
			}
			catch (Exception exp)
			{
				logger.Error(exp);
			}
		}

		/// <summary>
		/// Execute closing function and persist session data to be reloaded on next restart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnClosed(Window win)
		{
			try
			{
				// Persist window position, width and height from this session
				SettingsManager.Instance.SessionData.MainWindowPosSz =
					new ViewPosSizeModel(win.Left, win.Top, win.Width, win.Height,
															 (win.WindowState == WindowState.Maximized ? true : false));

				// Save/initialize program options that determine global programm behaviour
				this.SaveConfigOnAppClosed();
			}
			catch (Exception exp)
			{
				logger.Error(exp);
				MsgBox.Msg.Show(exp.ToString(),
												Util.Local.Strings.STR_MSG_UnknownError_InShutDownProcess,
												MsgBoxButtons.OK, MsgBoxImage.Error);
			}
		}
	}
}
