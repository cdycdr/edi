namespace EdiViews.ViewModel.Base
{
  using MsgBox;
  using SimpleControls.Command;
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Input;
  using Edi.Core.ViewModels.Events;
  using Edi.Core.ViewModels;

  /// <summary>
  /// Base class that shares common properties, methods, and intefaces
  /// among viewmodels that represent documents in Edi
  /// (text file edits, Start Page, Prgram Settings).
  /// </summary>
  public abstract class FileBaseViewModel : PaneViewModel
  {
    #region Fields
    private bool mIsFilePathReal = false;
    
    private RelayCommand<object> _openContainingFolderCommand = null;
    private RelayCommand<object> _copyFullPathtoClipboard = null;
    private RelayCommand<object> _syncPathToExplorerCommand = null;
    #endregion Fields

    #region events
    /// <summary>
    /// This event is fired when a document tells the framework that is wants to be closed.
    /// The framework can then close it and clean-up whatever is left to clean-up.
    /// </summary>
    virtual public event EventHandler<FileBaseEvent> DocumentEvent;
    #endregion events

    #region properties
    #region IsFilePathReal
    /// <summary>
    /// Get/set whether a given file path is a real existing path or not.
    /// 
    /// This is used to identify files that have never been saved and can
    /// those not be remembered in an MRU etc...
    /// </summary>
    public bool IsFilePathReal
    {
      get
      {
        return this.mIsFilePathReal;
      }

      set
      {
        this.mIsFilePathReal = value;
      }
    }
    #endregion IsFilePathReal

    #region FilePath
    abstract public string FilePath{ get; protected set; }
    #endregion FilePath

    #region FileName
    /// <summary>
    /// FileName is the string that is displayed whenever the application refers to this file, as in:
    /// string.Format(CultureInfo.CurrentCulture, "Would you like to save the '{0}' file", FileName)
    /// 
    /// Note the absense of the dirty mark '*'. Use the Title property if you want to display the file
    /// name with or without dirty mark when the user has edited content.
    /// </summary>
    abstract public string FileName { get; }
    #endregion FileName

    #region IsDirty
    /// <summary>
    /// Get whether the current information was edit and needs to be saved or not.
    /// </summary>
    abstract public bool IsDirty{ get; set; }
    #endregion IsDirty

    #region CanSaveData
    /// <summary>
    /// Get whether edited data can be saved or not.
    /// A type of document does not have a save
    /// data implementation if this property returns false.
    /// (this is document specific and should always be overriden by descendents)
    /// </summary>
    abstract public bool CanSaveData{ get; }
    #endregion CanSaveData

    #region Commands
    /// <summary>
    /// This command cloases a single file. The binding for this is in the AvalonDock LayoutPanel Style.
    /// </summary>
    abstract public ICommand CloseCommand
    {
      get;
    }

    /// <summary>
    /// Get open containing folder command which will open
    /// the folder indicated by the path in windows explorer
    /// and select the file (if path points to one).
    /// </summary>
    public ICommand OpenContainingFolderCommand
    {
      get
      {
        if (_openContainingFolderCommand == null)
          _openContainingFolderCommand = new RelayCommand<object>((p) => this.OnOpenContainingFolderCommand());

        return _openContainingFolderCommand;
      }
    }


    /// <summary>
    /// Get CopyFullPathtoClipboard command which will copy
    /// the path of the executable into the windows clipboard.
    /// </summary>
    public ICommand CopyFullPathtoClipboard
    {
      get
      {
        if (_copyFullPathtoClipboard == null)
          _copyFullPathtoClipboard = new SimpleControls.Command.RelayCommand<object>((p) => this.OnCopyFullPathtoClipboardCommand());

        return _copyFullPathtoClipboard;
      }
    }

    /// <summary>
    /// 
    /// </summary>
    public ICommand SyncPathToExplorerCommand
    {
      get
      {
        if (this._syncPathToExplorerCommand == null)
          this._syncPathToExplorerCommand = new SimpleControls.Command.RelayCommand<object>(
                                                  (p) => this.OnSyncPathToExplorerCommand());

        return this._syncPathToExplorerCommand;
      }
    }
    #endregion commands
  	#endregion properties

    #region methods
    #region abstract methods
    /// <summary>
    /// Indicate whether document can be saved in the currennt state.
    /// </summary>
    /// <returns></returns>
    abstract public bool CanSave();

    /// <summary>
    /// Indicate whether document can be saved as.
    /// </summary>
    /// <returns></returns>
    abstract public bool CanSaveAs();

    /// <summary>
    /// Save this document as.
    /// </summary>
    /// <returns></returns>
    abstract public bool SaveFile(string filePath);

    /// <summary>
    /// Return the path of the file representation (if any).
    /// </summary>
    /// <returns></returns>
    abstract public string GetFilePath();
    #endregion abstract methods

    /// <summary>
    /// Search for most inner exceptions and return it to caller.
    /// </summary>
    /// <param name="exp"></param>
    /// <param name="caption"></param>
    /// <returns></returns>
    public Exception GetInnerMostException(Exception exp)
    {
      if (exp != null)
      {
        while (exp.InnerException != null)
          exp = exp.InnerException;
      }

      if (exp != null)
        return exp;

      return null;
    }

    /// <summary>
    /// Get a path that does not represent this document that indicates
    /// a useful alternative representation (eg: StartPage -> Assembly Path).
    /// </summary>
    /// <returns></returns>
    public string GetAlternativePath()
    {
      return this.FilePath;
    }

    /// <summary>
    /// This method is executed to tell the surrounding framework to close the document.
    /// </summary>
    protected virtual void OnClose()
    {
      if (this.DocumentEvent != null)
        this.DocumentEvent(this, new FileBaseEvent(FileEventType.CloseDocument));
    }

    /// <summary>
    /// Indicate whether document can be closed or not.
    /// </summary>
    /// <returns></returns>
    public virtual bool CanClose()
    {
      return (this.DocumentEvent != null);
    }
    
    private void OnCopyFullPathtoClipboardCommand()
    {
      try
      {
        System.Windows.Clipboard.SetText(this.FilePath);
      }
      catch
      {
      }
    }

    /// <summary>
    /// Opens the folder in which this document is stored in the Windows Explorer.
    /// </summary>
    private void OnOpenContainingFolderCommand()
    {
      try
      {
        if (System.IO.File.Exists(this.FilePath) == true)
        {
          // combine the arguments together it doesn't matter if there is a space after ','
          string argument = @"/select, " + this.FilePath;

          System.Diagnostics.Process.Start("explorer.exe", argument);
        }
        else
        {
          string parentDir = System.IO.Directory.GetParent(this.FilePath).FullName;

          if (System.IO.Directory.Exists(parentDir) == false)
            MsgBox.Msg.Show(string.Format(CultureInfo.CurrentCulture, Util.Local.Strings.STR_ACCESS_DIRECTORY_ERROR, parentDir),
                            Util.Local.Strings.STR_FILE_FINDING_CAPTION,
                            MsgBoxButtons.OK, MsgBoxImage.Error);
          else
          {
            string argument = @"/select, " + parentDir;

            System.Diagnostics.Process.Start("EXPLORER.EXE", argument);
          }
        }
      }
      catch (System.Exception ex)
      {
        MsgBox.Msg.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (this.FilePath == null ? string.Empty : this.FilePath)),
                        Util.Local.Strings.STR_FILE_FINDING_CAPTION,
                        MsgBoxButtons.OK, MsgBoxImage.Error);
      }
    }

    /// <summary>
    /// Synchronizes the path of the Explorer Tool Window with
    /// the path of this document.
    /// </summary>
    private void OnSyncPathToExplorerCommand()
    {
      try
      {
        if (this.DocumentEvent != null)
          this.DocumentEvent(this, new FileBaseEvent(FileEventType.AdjustCurrentPath));
      }
      catch
      {
      }
    }
    #endregion methods
  }
}
