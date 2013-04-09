﻿namespace EdiViews.ViewModel.Base
{
  using System.Windows.Input;

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

    abstract public bool IsDirty{ get; set; }

    #region CloseCommand
    /// <summary>
    /// This command cloases a single file. The binding for this is in the AvalonDock LayoutPanel Style.
    /// </summary>
    abstract public ICommand CloseCommand
    {
      get;
    }
    #endregion
    #endregion properties

    #region methods
    abstract public bool CanClose();
    abstract public bool CanSave();
    abstract public bool CanSaveAs();
    abstract public bool OnSaveAs();
    abstract public string GetFilePath();
    #endregion methods
  }
}
