namespace EdiViews.Tools.FileStats
{
  using System;
  using System.IO;
  using Edi.Core.ViewModels;

  /// <summary>
  /// This class can be used to present file based information, such as,
  /// Size, Path etc to the user.
  /// </summary>
  public class FileStatsViewModel : Edi.Core.ViewModels.ToolViewModel
  {
    #region fields
    public const string ToolContentId = "FileStatsTool";
    private string mFilePathName;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public FileStatsViewModel()
      : base("File Info")
    {
      ContentId = ToolContentId;
    }
    #endregion constructor

    #region properties
    #region FileSize

    private long _fileSize;
    public long FileSize
    {
      get { return _fileSize; }
      set
      {
        if (_fileSize != value)
        {
          _fileSize = value;
          RaisePropertyChanged("FileSize");
        }
      }
    }

    #endregion

    #region LastModified
    private DateTime _lastModified;
    public DateTime LastModified
    {
      get { return _lastModified; }
      set
      {
        if (_lastModified != value)
        {
          _lastModified = value;
          RaisePropertyChanged("LastModified");
        }
      }
    }
    #endregion

    #region FileName
    public string FileName
    {
      get
      {
        if (string.IsNullOrEmpty(this.mFilePathName) == true)
          return string.Empty;

        try
        {
          return System.IO.Path.GetFileName(mFilePathName);
        }
        catch (Exception)
        {
          return string.Empty;
        }
      }
    }
    #endregion

    #region FilePath
    public string FilePath
    {
      get
      {
        if (string.IsNullOrEmpty(this.mFilePathName) == true)
          return string.Empty;

        try
        {
          return System.IO.Path.GetDirectoryName(mFilePathName);
        }
        catch (Exception)
        {
          return string.Empty;
        }
      }
    }
    #endregion

    #region ToolWindow Icon
    public override Uri IconSource
    {
      get
      {
        return new Uri("pack://application:,,,/Edi;component/Images/property-blue.png", UriKind.RelativeOrAbsolute);
      }
    }
    #endregion ToolWindow Icon
    #endregion properties

    #region methods
    public void OnActiveDocumentChanged(object sender, DocumentChangedEventArgs e)
    {
      this.mFilePathName = string.Empty;
      FileSize = 0;
      LastModified = DateTime.MinValue;

      if (e != null)
      {
        if (e.ActiveDocument != null)
        {
          FileBaseViewModel f = e.ActiveDocument as FileBaseViewModel;

          if (f != null)
          {
            if (File.Exists(f.FilePath) == true)
            {
              var fi = new FileInfo(f.FilePath);

              this.mFilePathName = f.FilePath;

              this.RaisePropertyChanged(() => this.FileName);
              this.RaisePropertyChanged(() => this.FilePath);

              FileSize = fi.Length;
              LastModified = fi.LastWriteTime;
            }
          }
        }
      }
    }
    #endregion methods
  }
}
