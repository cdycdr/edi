namespace EdiViews.ViewModel.Base
{
  using System.Windows.Input;
  using System;
  using MsgBox;
  using System.Globalization;
  using SimpleControls.Command;

  /// <summary>
  /// Base class that shares common properties, methods, and intefaces
  /// among viewmodels that represent documents in Edi
  /// (text file edits, Start Page, Prgram Settings).
  /// </summary>
  public abstract class FileBaseViewModel : EdiViews.ViewModel.Base.PaneViewModel
  {
    #region Fields
    private bool mIsFilePathReal = false;
    #endregion Fields

    /// <summary>
    /// This event is fired when a document tells the framework that is wants to be closed.
    /// The framework can then close it and clean-up whatever is left to clean-up.
    /// </summary>
    virtual public event EventHandler CloseDocument;

    #region properties
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

    abstract public string FilePath{ get; protected set; }

    abstract public bool IsDirty{ get; protected set; }

    #region CloseCommand
    /// <summary>
    /// This command cloases a single file. The binding for this is in the AvalonDock LayoutPanel Style.
    /// </summary>
    abstract public ICommand CloseCommand
    {
      get;
    }
    #endregion

    #region OpenContainingFolder
    RelayCommand<object> _openContainingFolderCommand = null;
    public ICommand OpenContainingFolderCommand
    {
      get
      {
        if (_openContainingFolderCommand == null)
          _openContainingFolderCommand = new RelayCommand<object>((p) => this.OnOpenContainingFolderCommand(),
                                                                  (p) => this.CanOpenContainingFolderCommand());

        return _openContainingFolderCommand;
      }
    }

    public bool CanOpenContainingFolderCommand()
    {
      return true;
    }

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
            MsgBox.Msg.Box.Show(string.Format(CultureInfo.CurrentCulture, "The directory '{0}' does not exist or cannot be accessed.", parentDir),
                                "Error finding file", MsgBoxButtons.OK, MsgBoxImage.Error);
          else
          {
            string argument = @"/select, " + parentDir;

            System.Diagnostics.Process.Start("EXPLORER.EXE", argument);
          }
        }
      }
      catch (System.Exception ex)
      {
        MsgBox.Msg.Box.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (this.FilePath == null ? string.Empty : this.FilePath)),
                            "Error finding file:", MsgBoxButtons.OK, MsgBoxImage.Error);
      }
    }
    #endregion OpenContainingFolder

    #region CopyFullPathtoClipboard
    RelayCommand<object> _copyFullPathtoClipboard = null;
    public ICommand CopyFullPathtoClipboard
    {
      get
      {
        if (_copyFullPathtoClipboard == null)
          _copyFullPathtoClipboard = new SimpleControls.Command.RelayCommand<object>((p) => this.OnCopyFullPathtoClipboardCommand(), (p) => this.CanCopyFullPathtoClipboardCommand());

        return _copyFullPathtoClipboard;
      }
    }

    public bool CanCopyFullPathtoClipboardCommand()
    {
      return true;
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
    #endregion CopyFullPathtoClipboard
    #endregion properties

    #region methods
    /// <summary>
    /// Indicate whether document can be closed.
    /// </summary>
    /// <returns></returns>
    abstract public bool CanClose();

    /// <summary>
    /// Indicate whether document can be saved.
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
    abstract public bool OnSaveAs();

    /// <summary>
    /// Return the path of the file representation (if any).
    /// </summary>
    /// <returns></returns>
    abstract public string GetFilePath();

    /// <summary>
    /// This method is executed to tell the surrounding framework to close the document.
    /// </summary>
    protected void OnClose()
    {
      if (this.CloseDocument != null)
        this.CloseDocument(this, EventArgs.Empty);
    }
    #endregion methods
  }
}
