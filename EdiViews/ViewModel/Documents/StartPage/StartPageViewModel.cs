﻿namespace EdiViews.Documents.StartPage
{
  using System;
  using System.Windows.Input;
  using SimpleControls.Command;
  using SimpleControls.MRU.ViewModel;
  using EdiViews.ViewModel;
  using System.Reflection;
  using System.Globalization;
  using MsgBox;

  public class StartPageViewModel : EdiViews.ViewModel.Base.FileBaseViewModel
  {
    #region fields
    public const string StartPageContentId = "{StartPage}";

    private MRUListVM mRecent;
    #endregion fields

    #region constructor
    /// <summary>
    /// Default constructor
    /// </summary>
    public StartPageViewModel()
    {
      this.Title = "Start Page";
      this.StartPageTip = "Welcome to Edi. Review this page to get started with this editor application.";
      this.ContentId = StartPageViewModel.StartPageContentId;
    }

    /// <summary>
    /// Parameterized constructor
    /// </summary>
    public StartPageViewModel(MRUListVM recent)
      : this()
    {
      this.mRecent = recent;
    }
    #endregion constructor

    #region properties
    #region CloseCommand
    RelayCommand<object> _closeCommand = null;
    public override ICommand CloseCommand
    {
      get
      {
        if (_closeCommand == null)
          _closeCommand = new RelayCommand<object>((p) => this.OnClose(), (p) => this.CanClose());

        return _closeCommand;
      }
    }

    override public bool CanClose()
    {
      return true;
    }
    #endregion

    #region OpenContainingFolder
    private RelayCommand<object> _openContainingFolderCommand = null;

    /// <summary>
    /// Get open containing folder command which will open
    /// the folder containing the executable in windows explorer
    /// and select the file.
    /// </summary>
    public new ICommand OpenContainingFolderCommand
    {
      get
      {
        if (_openContainingFolderCommand == null)
          _openContainingFolderCommand = new RelayCommand<object>((p) => this.OnOpenContainingFolderCommand());

        return _openContainingFolderCommand;
      }
    }

    private void OnOpenContainingFolderCommand()
    {
      try
      {
        // combine the arguments together it doesn't matter if there is a space after ','
        string argument = @"/select, " + this.GetEntryAssemblyPath();

        System.Diagnostics.Process.Start("explorer.exe", argument);
      }
      catch (System.Exception ex)
      {
        MsgBox.Msg.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (this.FilePath == null ? string.Empty : this.FilePath)),
                            "Error finding file:", MsgBoxButtons.OK, MsgBoxImage.Error);
      }
    }
    #endregion OpenContainingFolder

    #region CopyFullPathtoClipboard
    private RelayCommand<object> _copyFullPathtoClipboard = null;

    /// <summary>
    /// Get CopyFullPathtoClipboard command which will copy
    /// the path of the executable into the windows clipboard.
    /// </summary>
    public new ICommand CopyFullPathtoClipboard
    {
      get
      {
        if (_copyFullPathtoClipboard == null)
          _copyFullPathtoClipboard = new SimpleControls.Command.RelayCommand<object>((p) => this.OnCopyFullPathtoClipboardCommand());

        return _copyFullPathtoClipboard;
      }
    }

    private void OnCopyFullPathtoClipboardCommand()
    {
      try
      {
        System.Windows.Clipboard.SetText(this.GetEntryAssemblyPath());
      }
      catch
      {
      }
    }
    #endregion CopyFullPathtoClipboard

    public override Uri IconSource
    {
      get
      {
        // This icon is visible in AvalonDock's Document Navigator window
        return new Uri("pack://application:,,,/Edi;component/Images/document.png", UriKind.RelativeOrAbsolute);
      }
    }

    public MRUListVM MruList
    {
      get
      {
        return this.mRecent;
      }
    }

    public string StartPageTip { get; set; }

    override public bool IsDirty
    {
      get
      {
        return false;
      }

      protected set
      {
        throw new NotSupportedException("Start page cannot be saved therfore setting dirty cannot be useful.");
      }
    }

    /// <summary>
    /// Get whether edited data can be saved or not.
    /// This type of document does not have a save
    /// data implementation if this property returns false.
    /// (this is document specific and should always be overriden by descendents)
    /// </summary>
    override public bool CanSaveData
    {
      get
      {
        return false;
      }
    }

    override public string FilePath
    {
      get
      {
        return this.ContentId;
      }

      protected set
      {
        throw new NotSupportedException();
      }
    }

    public override string FileName
    {
      get { return string.Empty; }
    }
    #endregion properties

    #region methods
    override public bool CanSave() { return false; }

    override public bool CanSaveAs() { return false; }

    override public bool SaveFile(string filePath)
    {
      throw new NotImplementedException();
    }

    override public string GetFilePath()
    {
      throw new NotSupportedException("Start Page does not have a valid file path.");
    }

    private string GetEntryAssemblyPath()
    {
      return Assembly.GetEntryAssembly().Location;
    }
    #endregion methods
  }
}
