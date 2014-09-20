﻿namespace EdiViews.ViewModel
{
	using System;
	using System.Windows;
	using Edi.Core.ViewModels;
	using MiniUML.Model.ViewModels.Document;
	using Settings.ProgramSettings;

	/// <summary>
	/// This interface models the viewmodel that manages the complete
	/// application life cyle from start to end. It publishes the methodes,
	/// properties, and events necessary to integrate the application into
	/// a given shell (BootStrapper, App.xaml.cs etc).
	/// </summary>
	public interface IApplicationViewModel : IMiniUMLDocument
	{
		/// <summary>
		/// Raised when this workspace should be removed from the UI.
		/// </summary>
		event EventHandler RequestClose;

		#region properties
		/// <summary>
		/// Get/set property to determine whether window is in maximized state or not.
		/// (this can be handy to determine when a resize grip should be shown or not)
		/// </summary>
		bool? IsNotMaximized { get; set; }

		bool ShutDownInProgress_Cancel { get; set; }
		#endregion properties

		#region methods
    /// <summary>
    /// Method to be executed when user (or program) tries to close the application
    /// </summary>
    void OnRequestClose();

		/// <summary>
		/// Save session data on closing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnClosing(object sender, System.ComponentModel.CancelEventArgs e);

		/// <summary>
		/// Execute closing function and persist session data to be reloaded on next restart
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void OnClosed(Window win);

		/// <summary>
		/// Check if pre-requisites for closing application are available.
		/// Save session data on closing and cancel closing process if necessary.
		/// </summary>
		/// <returns>true if application is OK to proceed closing with closed, otherwise false.</returns>
		bool Exit_CheckConditions(object sender);

		/// <summary>
		/// Load configuration from persistence on startup of application
		/// </summary>
		void LoadConfigOnAppStartup(Options programSettings);

    /// <summary>
    /// Save application settings when the application is being closed down
    /// </summary>
    void SaveConfigOnAppClosed();

		/// <summary>
		/// Bind a window to some commands to be executed by the viewmodel.
		/// </summary>
		/// <param name="win"></param>
		void InitCommandBinding(Window win);

    /// <summary>
    /// Open a file supplied in <paramref name="filePath"/> (without displaying a file open dialog).
    /// </summary>
    /// <param name="filePath">file to open</param>
    /// <param name="AddIntoMRU">indicate whether file is to be added into MRU or not</param>
    /// <returns></returns>
    FileBaseViewModel Open(string filePath,
                           CloseDocOnError closeDocumentWithoutMessageOnError = CloseDocOnError.WithUserNotification,
                           bool AddIntoMRU = true,
                           TypeOfDocument t = TypeOfDocument.EdiTextEditor);
		#endregion methods
	}
}
