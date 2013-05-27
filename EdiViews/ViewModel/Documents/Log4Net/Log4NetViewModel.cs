namespace EdiViews.Documents.Log4Net
{
  using System;
  using System.Globalization;
  using System.Windows.Input;
  using MsgBox.Commands;
  using System.IO;
  using MsgBox;
  using ICSharpCode.AvalonEdit.Utils;
  using System.Text;

  public class Log4NetViewModel : EdiViews.ViewModel.Base.FileBaseViewModel
  {
    #region fields
    private static int iNewFileCounter = 1;
    private string defaultFileType = "log4net";
    private string defaultFileName = "Untitled";

    private YalvLib.ViewModel.YalvViewModel mYalvVM = null;
    #endregion fields

    #region constructor
    public Log4NetViewModel()
    {
      this.Title = "Start Page";
      this.ScreenTip = "A log4net document can be used to view the XML output from a log4net logger session.";
      this.ContentId = string.Empty;
      this.IsReadOnlyReason = "Log4net logger output cannot be edit in EDI. This is a read-only viewer document.";

      this.FilePath = string.Format(CultureInfo.InvariantCulture, "{0} {1}.{2}", this.defaultFileName,
                                    Log4NetViewModel.iNewFileCounter++,
                                    this.defaultFileType);

      this.mYalvVM = new YalvLib.ViewModel.YalvViewModel();
    }
    #endregion constructor

    #region properties
    public string ScreenTip { get; set; }

    public string IsReadOnlyReason { get; set; }

    #region FilePath
    private string mFilePath = null;

    /// <summary>
    /// Get/set complete path including file name to where this stored.
    /// This string is never null or empty.
    /// </summary>
    override public string FilePath
    {
      get
      {
        if (this.mFilePath == null || this.mFilePath == String.Empty)
          return string.Format(CultureInfo.CurrentCulture, "New.{1}", this.defaultFileType);

        return this.mFilePath;
      }

      protected set
      {
        if (this.mFilePath != value)
        {
          this.mFilePath = value;

          this.NotifyPropertyChanged(() => this.FilePath);
          this.NotifyPropertyChanged(() => this.FileName);
          this.NotifyPropertyChanged(() => this.Title);
        }
      }
    }
    #endregion

    #region Title
    /// <summary>
    /// Title is the string that is usually displayed - with or without dirty mark '*' - in the docking environment
    /// </summary>
    public override string Title
    {
      get
      {
        return this.FileName + (this.IsDirty == true ? "*" : string.Empty);
      }
    }
    #endregion

    #region FileName
    /// <summary>
    /// FileName is the string that is displayed whenever the application refers to this file, as in:
    /// string.Format(CultureInfo.CurrentCulture, "Would you like to save the '{0}' file", FileName)
    /// 
    /// Note the absense of the dirty mark '*'. Use the Title property if you want to display the file
    /// name with or without dirty mark when the user has edited content.
    /// </summary>
    public override string FileName
    {
      get
      {
        // This option should never happen - its an emergency break for those cases that never occur
        if (FilePath == null || FilePath == String.Empty)
          return string.Format(CultureInfo.InvariantCulture, "{0}.{1}", this.defaultFileName, this.defaultFileType);

        return System.IO.Path.GetFileName(FilePath);
      }
    }

    public override Uri IconSource
    {
      get
      {
        // This icon is visible in AvalonDock's Document Navigator window
        return new Uri("pack://application:,,,/EdiViews;component/Images/Documents/Log4net.png", UriKind.RelativeOrAbsolute);
      }
    }
    #endregion FileName

    #region DocumentCommands
    /// <summary>
    /// IsDirty indicates whether the file currently loaded
    /// in the editor was modified by the user or not
    /// (this should always be false since log4net documents cannot be edit and saved).
    /// </summary>
    override public bool IsDirty
    {
      get
      {
        return false;
      }

      protected set
      {
        throw new NotSupportedException("Log4Net documents cannot be saved therfore setting dirty cannot be useful.");
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

    override public bool CanSave() { return false; }

    override public bool CanSaveAs() { return false; }

    override public bool SaveFile(string filePath)
    {
      throw new NotImplementedException();
    }

    #region CloseCommand
    RelayCommand<object> _closeCommand = null;
    public override ICommand CloseCommand
    {
      get
      {
        if (_closeCommand == null)
        {
          _closeCommand = new RelayCommand<object>((p) => OnClose(), (p) => CanClose());
        }

        return _closeCommand;
      }
    }

    override public bool CanClose()
    {
      return true;
    }
    #endregion
    #endregion DocumentCommands

    public YalvLib.ViewModel.YalvViewModel Yalv
    {
      get
      {
        return this.mYalvVM;
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Get the path of the file or empty string if file does not exists on disk.
    /// </summary>
    /// <returns></returns>
    override public string GetFilePath()
    {
      try
      {
        if (System.IO.File.Exists(this.FilePath))
          return System.IO.Path.GetDirectoryName(this.FilePath);
      }
      catch
      {
      }

      return string.Empty;
    }
    #endregion

    /// <summary>
    /// Load a log4net file and return the corresponding viewmodel representation for it.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static Log4NetViewModel LoadFile(string filePath)
    {
      bool IsFilePathReal = false;

      try
      {
        IsFilePathReal = File.Exists(filePath);
      }
      catch
      {
      }

      if (IsFilePathReal == false)
        return null;

      Log4NetViewModel vm = new Log4NetViewModel();

      if (vm.OpenFile(filePath) == true)
        return vm;

      return null;
    }

    /// <summary>
    /// Attempt to open a file and load it into the viewmodel if it exists.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns>True if file exists and was succesfully loaded. Otherwise false.</returns>
    protected bool OpenFile(string filePath)
    {
      try
      {
        if ((this.IsFilePathReal = File.Exists(filePath)) == true)
        {
          this.FilePath = filePath;
          this.ContentId = this.mFilePath;

          // File may be blocked by another process
          // Try read-only shared method and set file access to read-only
          try
          {
            this.mYalvVM.LoadFile(filePath);
          }
          catch (Exception ex)
          {
            MsgBox.Msg.Show(ex.Message, "An error has occurred", MsgBoxButtons.OK);

            return false;
          }
        }
        else
          return false;
      }
      catch (Exception exp)
      {
        MsgBox.Msg.Show(exp.Message, "An error has occurred", MsgBoxButtons.OK);

        return false;
      }

      return true;
    }

  }
}
