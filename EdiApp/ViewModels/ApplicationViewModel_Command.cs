﻿namespace EdiApp.ViewModels
{
	using System;
	using System.Diagnostics;
	using System.Threading;
	using System.Windows;
	using System.Windows.Input;
	using System.Windows.Threading;
	using Edi.Core;
	using Edi.Core.Interfaces;
	using Edi.Core.ViewModels;
	using Edi.Core.ViewModels.Command;
	using EdiApp.Enums;
	using EdiDocuments.ViewModels;
	using EdiDocuments.ViewModels.EdiDoc;
	using EdiDocuments.ViewModels.StartPage;
	using MiniUML.Framework;
	using MsgBox;
	using Themes;

	public partial class ApplicationViewModel
	{
		private bool Closing_CanExecute()
		{
			if (this.mShutDownInProgress == true)
				return false;

			// Check if conditions within the WorkspaceViewModel are suitable to close the application
			// eg.: Prompt to Cancel long running background tasks such as Search - Replace in Files (if any)

			return true;
		}

		/// <summary>
		/// Bind a window to some commands to be executed by the viewmodel.
		/// </summary>
		/// <param name="win"></param>
		public void InitCommandBinding(Window win)
		{
			this.InitEditCommandBinding(win);

			win.CommandBindings.Add(new CommandBinding(AppCommand.Exit,
			(s, e) =>
			{
				this.AppExit_CommandExecuted();
				e.Handled = true;
			}));

			win.CommandBindings.Add(new CommandBinding(AppCommand.About,
			(s, e) =>
			{
				this.AppAbout_CommandExecuted();
				e.Handled = true;
			}));

			win.CommandBindings.Add(new CommandBinding(AppCommand.ProgramSettings,
			(s, e) =>
			{
				this.AppProgramSettings_CommandExecuted();
				e.Handled = true;
			}));

			win.CommandBindings.Add(new CommandBinding(AppCommand.ShowToolWindow,
			(s, e) =>
			{
				if (e == null)
					return;

				var toolwindowviewmodel = e.Parameter as IToolWindow;

				if (toolwindowviewmodel == null)
					return;

				var registerTW = toolwindowviewmodel as IRegisterableToolWindow;

				if (registerTW != null)
					registerTW.SetToolWindowVisibility(this, !toolwindowviewmodel.IsVisible);
				else
					toolwindowviewmodel.SetToolWindowVisibility(!toolwindowviewmodel.IsVisible);

				e.Handled = true;
			}));

			// Standard File New command binding via ApplicationCommands enumeration
			win.CommandBindings.Add(new CommandBinding(ApplicationCommands.New,
			(s, e) =>
			{
				TypeOfDocument t = TypeOfDocument.EdiTextEditor;

				if (e != null)
				{
					e.Handled = true;

					if (e.Parameter != null)
					{
						if (e.Parameter is TypeOfDocument)
							t = (TypeOfDocument)e.Parameter;
					}
				}

				this.OnNew(t);
			}
			));

			// Standard File Open command binding via ApplicationCommands enumeration
			win.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open,
			(s, e) =>
			{
				string t = string.Empty;

				if (e != null)
				{
					if (e.Parameter != null)
					{
						if (e.Parameter is string)
							t = (string)e.Parameter;
					}
				}

				this.OnOpen(t);
				e.Handled = true;
			}
			));

			// Close Document command
			// Closes the FileViewModel document supplied in e.parameter
			// or the Active document
			win.CommandBindings.Add(new CommandBinding(AppCommand.CloseFile,
			(s, e) =>
			{
				try
				{
					FileBaseViewModel f = null;

					if (e != null)
					{
						e.Handled = true;
						f = e.Parameter as FileBaseViewModel;
					}

					if (f != null)
						this.Close(f);
					else
					{
						if (this.ActiveDocument != null)
							this.Close(this.ActiveDocument);
					}
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			},
			(s, e) =>
			{
				try
				{
					if (e != null)
					{
						e.Handled = true;
						e.CanExecute = false;

						EdiViewModel f = null;

						if (e != null)
						{
							e.Handled = true;
							f = e.Parameter as EdiViewModel;
						}

						if (f != null)
							e.CanExecute = f.CanClose();
						else
						{
							if (this.ActiveDocument != null)
								e.CanExecute = this.ActiveDocument.CanClose();
						}
					}
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			}));

			// Change the WPF/TextEditor highlighting theme currently used in the application
			win.CommandBindings.Add(new CommandBinding(AppCommand.ViewTheme,
															(s, e) => this.ChangeThemeCmd_Executed(s, e, win.Dispatcher)));

			win.CommandBindings.Add(new CommandBinding(AppCommand.BrowseURL,
			(s, e) =>
			{
				Process.Start(new ProcessStartInfo("http://Edi.codeplex.com"));
			}));

			win.CommandBindings.Add(new CommandBinding(AppCommand.ShowStartPage,
			(s, e) =>
			{
				StartPageViewModel spage = this.GetStartPage(true);

				if (spage != null)
				{
					logger.InfoFormat("TRACE Before setting startpage as ActiveDocument");
					this.ActiveDocument = spage;
					logger.InfoFormat("TRACE After setting startpage as ActiveDocument");
				}
			}));

			win.CommandBindings.Add(new CommandBinding(AppCommand.ToggleOptimizeWorkspace,
			(s, e) =>
			{
				logger.InfoFormat("TRACE AppCommand.ToggleOptimizeWorkspace parameter is {0}.", (e == null ? "(null)" : e.ToString()));

				try
				{
					var newViewSetting = !this.IsWorkspaceAreaOptimized;
					this.IsWorkspaceAreaOptimized = newViewSetting;
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			}));

			win.CommandBindings.Add(new CommandBinding(AppCommand.LoadFile,
			(s, e) =>
			{
				try
				{
					logger.InfoFormat("TRACE AppCommand.LoadFile parameter is {0}.", (e == null ? "(null)" : e.ToString()));

					if (e == null)
						return;

					string filename = e.Parameter as string;

					if (filename == null)
						return;

					logger.InfoFormat("TRACE AppCommand.LoadFile with: '{0}'", filename);

					this.Open(filename);
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			}));

			win.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save,
			(s, e) =>
			{
				try
				{
					if (e != null)
						e.Handled = true;

					if (this.ActiveDocument != null)
						this.OnSave(this.ActiveDocument, false);
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
													MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			},
			(s, e) =>
			{
				if (e != null)
				{
					e.Handled = true;

					if (this.ActiveDocument != null)
						e.CanExecute = this.ActiveDocument.CanSave();
				}
			}));

			win.CommandBindings.Add(new CommandBinding(ApplicationCommands.SaveAs,
			(s, e) =>
			{
				try
				{
					if (e != null)
						e.Handled = true;

					if (this.ActiveDocument != null)
					{
						if (this.OnSave(this.ActiveDocument, true))
						{
							this.mSettingsManager.SessionData.MruList.AddMRUEntry(this.ActiveDocument.FilePath);
							this.mSettingsManager.SessionData.LastActiveFile = this.ActiveDocument.FilePath;
						}
					}
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			},
			(s, e) =>
			{
				try
				{
					if (e != null)
					{
						e.Handled = true;
						e.CanExecute = false;

						if (this.ActiveDocument != null)
							e.CanExecute = this.ActiveDocument.CanSaveAs();
					}
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			}
			));

			// Execute a command to save all edited files and current program settings
			win.CommandBindings.Add(new CommandBinding(AppCommand.SaveAll,
			(s, e) =>
			{
				try
				{
					// Save all edited documents
					if (this.mFiles != null)               // Close all open files and make sure there are no unsaved edits
					{                                     // If there are any: Ask user if edits should be saved
						IFileBaseViewModel activeDoc = this.ActiveDocument;

						try
						{
							for (int i = 0; i < this.Files.Count; i++)
							{
								IFileBaseViewModel f = this.Files[i];

								if (f != null)
								{
									if (f.IsDirty == true && f.CanSaveData == true)
									{
										this.ActiveDocument = f;
										this.OnSave(f);
									}
								}
							}
						}
						catch (Exception exp)
						{
							MsgBox.Msg.Show(exp.ToString(), Util.Local.Strings.STR_MSG_UnknownError_Caption, MsgBoxButtons.OK);
						}
						finally
						{
							if (activeDoc != null)
								this.ActiveDocument = activeDoc;
						}
					}

					// Save program settings
					this.SaveConfigOnAppClosed();
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			}));

			// Execute a command to export UML editor content as image
			win.CommandBindings.Add(new CommandBinding(AppCommand.ExportUMLToImage,
			(s, e) =>
			{
				try
				{
					if (this.vm_DocumentViewModel != null)
					{
						if ((this.vm_DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready) == true)
						{
							this.vm_DocumentViewModel.ExecuteExport(s, e, this.ActiveDocument.FileName + ".png");
						}
					}
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			},
			(s, e) =>  // Execute this command only if an UML document is currently active
			{
				if (this.vm_DocumentViewModel != null)
					e.CanExecute = (this.vm_DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready);
				else
					e.CanExecute = false;
			}
			));

			// Execute a command to export Text editor content as highlighted image content
			win.CommandBindings.Add(new CommandBinding(AppCommand.ExportTextToHTML,
			(s, e) =>
			{
				try
				{
					if (this.ActiveEdiDocument != null)
						this.ActiveEdiDocument.ExportToHTML(this.ActiveDocument.FileName + ".html",
																								this.mSettingsManager.SettingData.TextToHTML_ShowLineNumbers,
																								this.mSettingsManager.SettingData.TextToHTML_AlternateLineBackground);
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			},
			(s, e) =>  // Execute this command only if a Text document is currently active
			{
				if (this.ActiveEdiDocument != null)
					e.CanExecute = true;
				else
					e.CanExecute = false;
			}
			));

			win.CommandBindings.Add(new CommandBinding(AppCommand.PinUnpin,
			(s, e) =>
			{
				this.PinCommand_Executed(e.Parameter, e);
			}));

			win.CommandBindings.Add(new CommandBinding(AppCommand.RemoveMruEntry,
			(s, e) =>
			{
				this.RemoveMRUEntry_Executed(e.Parameter, e);
			}));

			win.CommandBindings.Add(new CommandBinding(AppCommand.AddMruEntry,
			(s, e) =>
			{
				this.AddMRUEntry_Executed(e.Parameter, e);
			}));
		}

		/// <summary>
		/// This procedure changes the current WPF Application Theme into another theme
		/// while the application is running (re-boot should not be required).
		/// </summary>
		/// <param name="s"></param>
		/// <param name="e"></param>
		/// <param name="disp"></param>
		private void ChangeThemeCmd_Executed(object s,
																				 ExecutedRoutedEventArgs e,
																				 System.Windows.Threading.Dispatcher disp)
		{
			string oldTheme = ThemesManager.DefaultThemeName;

			try
			{
				if (e == null)
					return;

				if (e.Parameter == null)
					return;

				string newThemeName = e.Parameter as string;

				// Check if request is available
				if (newThemeName == null)
					return;

				oldTheme = this.mSettingsManager.SettingData.CurrentTheme;

				// The Work to perform on another thread
				ThreadStart start = delegate
				{
					// This works in the UI tread using the dispatcher with highest Priority
					disp.Invoke(DispatcherPriority.Send,
					(Action)(() =>
					{
						try
						{
							if (this.mThemesManager.SetSelectedTheme(newThemeName) == true)
							{
								this.mSettingsManager.SettingData.CurrentTheme = newThemeName;
								this.ResetTheme();                        // Initialize theme in process
							}
						}
						catch (Exception exp)
						{
							logger.Error(exp.Message, exp);
							MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
															this.mAppCore.IssueTrackerLink,
															this.mAppCore.IssueTrackerLink,
															Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
						}
					}));
				};

				// Create the thread and kick it started!
				Thread thread = new Thread(start);

				thread.Start();
			}
			catch (Exception exp)
			{
				this.mSettingsManager.SettingData.CurrentTheme = oldTheme;

				logger.Error(exp.Message, exp);
				MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
												this.mAppCore.IssueTrackerLink,
												this.mAppCore.IssueTrackerLink,
												Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
			}
		}

		#region EditorCommands

		/// <summary>
		/// Set command bindings necessary to perform copy/cut/paste operations
		/// </summary>
		/// <param name="win"></param>
		public void InitEditCommandBinding(Window win)
		{
			win.CommandBindings.Add(new CommandBinding(AppCommand.DisableHighlighting,    // Select all text in a document
			(s, e) =>
			{
				try
				{
					EdiViewModel f = this.ActiveDocument as EdiViewModel;

					if (f != null)
						f.DisableHighlighting();
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			},
			(s, e) =>
			{
				EdiViewModel f = this.ActiveDocument as EdiViewModel;

				if (f != null)
				{
					if (f.HighlightingDefinition != null)
					{
						e.CanExecute = true;
						return;
					}
				}

				e.CanExecute = false;
			}));

			#region GotoLine FindReplace
			win.CommandBindings.Add(new CommandBinding(AppCommand.GotoLine,    // Goto line n in a document
			(s, e) =>
			{
				try
				{
					e.Handled = true;

					this.ShowGotoLineDialog();
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			},
			(s, e) => { e.CanExecute = CanExecuteIfActiveDocumentIsEdiViewModel(); }));

			win.CommandBindings.Add(new CommandBinding(AppCommand.FindText,    // Find text in a document
			(s, e) =>
			{
				try
				{
					e.Handled = true;

					this.ShowFindReplaceDialog();
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			},
			(s, e) => { e.CanExecute = CanExecuteIfActiveDocumentIsEdiViewModel(); }));

			win.CommandBindings.Add(new CommandBinding(AppCommand.FindPreviousText,    // Find text in a document
			(s, e) =>
			{
				try
				{
					e.Handled = true;

					EdiViewModel f = this.ActiveDocument as EdiViewModel;

					if (f != null)
					{
						if (this.FindReplaceVM != null)
						{
							this.FindReplaceVM.FindNext(this.FindReplaceVM, true);
						}
						else
						{
							this.ShowFindReplaceDialog();
						}
					}
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			},
			(s, e) => { e.CanExecute = CanExecuteIfActiveDocumentIsEdiViewModel(); }));

			win.CommandBindings.Add(new CommandBinding(AppCommand.FindNextText,    // Find text in a document
			(s, e) =>
			{
				try
				{
					e.Handled = true;

					EdiViewModel f = this.ActiveDocument as EdiViewModel;

					if (f != null)
					{
						if (this.FindReplaceVM != null)
						{
							this.FindReplaceVM.FindNext(this.FindReplaceVM, false);
						}
						else
						{
							this.ShowFindReplaceDialog();
						}
					}
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			},
			(s, e) => { e.CanExecute = CanExecuteIfActiveDocumentIsEdiViewModel(); }));

			win.CommandBindings.Add(new CommandBinding(AppCommand.ReplaceText, // Find and replace text in a document
			(s, e) =>
			{
				try
				{
					e.Handled = true;

					this.ShowFindReplaceDialog(false);
				}
				catch (Exception exp)
				{
					logger.Error(exp.Message, exp);
					MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
													this.mAppCore.IssueTrackerLink,
													this.mAppCore.IssueTrackerLink,
													Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
				}
			},
			(s, e) => { e.CanExecute = CanExecuteIfActiveDocumentIsEdiViewModel(); }));
			#endregion GotoLine FindReplace
		}

		#region ToggleEditorOptionCommand
		RelayCommand<ToggleEditorOption> _toggleEditorOptionCommand = null;
		public ICommand ToggleEditorOptionCommand
		{
			get
			{
				if (this._toggleEditorOptionCommand == null)
				{
					this._toggleEditorOptionCommand = new RelayCommand<ToggleEditorOption>
										((p) => this.OnToggleEditorOption(p),
										 (p) => this.CanExecuteIfActiveDocumentIsEdiViewModel());
				}

				return this._toggleEditorOptionCommand;
			}
		}

		private void OnToggleEditorOption(object parameter)
		{
			EdiViewModel f = this.ActiveDocument as EdiViewModel;

			if (f == null)
				return;

			if (parameter == null)
				return;

			if ((parameter is ToggleEditorOption) == false)
				return;

			ToggleEditorOption t = (ToggleEditorOption)parameter;

			if (f != null)
			{
				switch (t)
				{
					case ToggleEditorOption.WordWrap:
						f.WordWrap = !f.WordWrap;
						break;

					case ToggleEditorOption.ShowLineNumber:
						f.ShowLineNumbers = !f.ShowLineNumbers;
						break;

					case ToggleEditorOption.ShowSpaces:
						f.TextOptions.ShowSpaces = !f.TextOptions.ShowSpaces;
						break;

					case ToggleEditorOption.ShowTabs:
						f.TextOptions.ShowTabs = !f.TextOptions.ShowTabs;
						break;

					case ToggleEditorOption.ShowEndOfLine:
						f.TextOptions.ShowEndOfLine = !f.TextOptions.ShowEndOfLine;
						break;

					default:
						break;
				}
			}
		}
		#endregion ToggleEditorOptionCommand

		private bool CanExecuteIfActiveDocumentIsEdiViewModel()
		{
			EdiViewModel f = this.ActiveDocument as EdiViewModel;

			if (f != null)
				return true;

			return false;
		}
		#endregion EditorCommands
	}
}
