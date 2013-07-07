namespace Edi.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Threading;

  using EdiViews.Config.ViewModel;
  using EdiViews.ViewModel.Base;
  using MsgBox;
  using MsgBox.Commands;
  using Util.Command;
  using EdiViews.StartPage;
  using EdiViews.Documents.StartPage;
  using EdiViews.ViewModel.Documents;
  using MiniUML.Framework;
using Microsoft.Win32;

  /// <summary>
  /// Base enumeration to determine the kind of command option
  /// to be executed on the text viewmodel bound to the text editor.
  /// 
  /// http://www.codeproject.com/Articles/570324/AvalonDock-2-0-Tutorial-Part-4-Integrating-AvalonE
  /// </summary>
  public enum ToggleEditorOption
  {
    WordWrap = 0,
    ShowLineNumber = 1,
    ShowEndOfLine = 2,
    ShowSpaces = 3,
    ShowTabs = 4
  }

  public enum TypeOfDocument
  {
    EdiTextEditor = 0,
    Log4NetView = 1,
    UMLEditor = 2,
  }

  public partial class Workspace
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
      }));

      win.CommandBindings.Add(new CommandBinding(AppCommand.About,
      (s, e) =>
      {
        this.AppAbout_CommandExecuted();
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
      },
      (s, e) =>
      {
        e.CanExecute = true;
      }
      ));

      // Standard File Open command binding via ApplicationCommands enumeration
      win.CommandBindings.Add(new CommandBinding(ApplicationCommands.Open,
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

        this.OnOpen(t);
      },
      (s, e) =>
      {
        e.CanExecute = true;
      }));

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
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
        }
      }));


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
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
            if (Workspace.This.OnSave(this.ActiveDocument, true))
              this.Config.MruList.AddMRUEntry(this.ActiveDocument.FilePath);
          }
        }
        catch (Exception exp)
        {
          logger.Error(exp.Message, exp);
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
            FileBaseViewModel activeDoc = this.ActiveDocument;

            try
            {
              for (int i = 0; i < this.Files.Count; i++)
              {
                FileBaseViewModel f = this.Files[i];

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
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
        }
      }));

      // Execute a command to save all edited files and current program settings
      win.CommandBindings.Add(new CommandBinding(AppCommand.ExportUMLToImage,
      (s, e) =>
      {
        try
        {
          if (this.vm_DocumentViewModel != null)
          {
            if ((this.vm_DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready) == true)
            {
              this.vm_DocumentViewModel.ExecuteExport(s, e, this.ActiveDocument.FileName);
            }
          }
        }
        catch (Exception exp)
        {
          logger.Error(exp.Message, exp);
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
        }
      },
      (s, e) =>  // Execute this comment only if an UML document is currently active
      {
        if (this.vm_DocumentViewModel != null)
          e.CanExecute = (this.vm_DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready);
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
    private void ChangeThemeCmd_Executed(object s, ExecutedRoutedEventArgs e, System.Windows.Threading.Dispatcher disp)
    {
      HlThemeKey oldTheme = new HlThemeKey(EdiThemesViewModel.WPFTheme.Generic);

      try
      {
        if (e == null)
        return;

        if (e.Parameter == null)
          return;

        HlThemeKey t = e.Parameter as HlThemeKey;

        // Check if request is available
        if (t == null)
          return;

        oldTheme = this.Config.CurrentTheme;

        // The Work to perform on another thread
        ThreadStart start = delegate
        {
          // This works in the UI tread using the dispatcher with highest Priority
          disp.Invoke(DispatcherPriority.Send,
          (Action)(() =>
          {
            try
            {

              this.Config.CurrentTheme = t;
            }
            catch (Exception exp)
            {
              logger.Error(exp.Message, exp);
              MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                              App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
            }
          }));
        };

        // Create the thread and kick it started!
        Thread thread = new Thread(start);

        thread.Start();
      }
      catch (Exception exp)
      {
        this.Config.CurrentTheme = oldTheme;

        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
          MsgBox.Msg.Show(exp, App.IssueTrackerText, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                          App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
