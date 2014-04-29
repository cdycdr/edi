namespace FolderBrowser.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.Linq;
  using FolderBrowser.Command;
  using FolderBrowser.Events;

  /// <summary>
  /// Source: http://www.codeproject.com/Articles/352874/WPF-Folder-Browser
  /// (but with RelayCommand instead of PRISM).
  /// </summary>
  public class BrowserViewModel : Base.ViewModelBase
  {
    #region fields
    private string mSelectedFolder;
    private bool mExpanding = false;

    private Environment.SpecialFolder mDesktopFolder = Environment.SpecialFolder.Desktop;
    private Environment.SpecialFolder mProgramFilesFolder = Environment.SpecialFolder.ProgramFiles;
    private Environment.SpecialFolder mMyDocumentsFolder = Environment.SpecialFolder.MyDocuments;
    private Environment.SpecialFolder mMyPicturesFolder = Environment.SpecialFolder.MyPictures;

    private RelayCommand<object> mFolderSelectedCommand = null;
    private RelayCommand<object> mSelectDirectoryCommand = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard <seealso cref="BrowserViewModel"/> constructor
    /// </summary>
    public BrowserViewModel()
    {
      this.Folders = new ObservableCollection<FolderViewModel>();
      Environment.GetLogicalDrives().ToList().ForEach(it => Folders.Add(
                                            new FolderViewModel { Root = this, FolderPath = it.TrimEnd('\\'),
                                                                  FolderName = it.TrimEnd('\\'),
                                                                  FolderIcon = "Images\\HardDisk.ico" }));
    }
    #endregion constructor
    
    #region events
    /// <summary>
    /// Event is fired to indicate that user wishes to select a certain path.
    /// </summary>
    public event EventHandler<SelectedDirectoryEventArgs> PathSelectionEvent;    
    #endregion events

    #region properties
    /// <summary>
    /// Get/set currently selected folder.
    /// </summary>
    public string SelectedFolder
    {
      get
      {
        return mSelectedFolder;
      }
      set
      {
        this.mSelectedFolder = value;
        this.RaisePropertyChanged(() => this.SelectedFolder);
        this.OnSelectedFolderChanged();
      }
    }

    public ObservableCollection<FolderViewModel> Folders
    {
      get;
      set;
    }

    /// <summary>
    /// Get/set command to select the current folder.
    /// </summary>
    public RelayCommand<object> FolderSelectedCommand
    {
      get
      {
        if (this.mFolderSelectedCommand == null)
          this.mFolderSelectedCommand = new RelayCommand<object>(it => this.SelectedFolder = Environment.GetFolderPath((Environment.SpecialFolder)it));

        //string spath = Environment.GetFolderPath(Environment.SpecialFolder.);

        return this.mFolderSelectedCommand;
      }
    }

    /// <summary>
    /// This command can be used to select a particular folder and tell the consumer
    /// of this viewmodel that the user wants to select this folder. The consumer can
    /// then diactivate the dialog or browse to the requested location using whatever
    /// is required outside of this control....
    /// </summary>
    public RelayCommand<object> SelectDirectoryCommand
    {
      get
      {
        if (this.mSelectDirectoryCommand == null)
          this.mSelectDirectoryCommand = new RelayCommand<object>(it =>
          {
            var path = it as FolderViewModel;

            if (path != null)
            {
              if (this.PathSelectionEvent != null)
                this.PathSelectionEvent(this, new SelectedDirectoryEventArgs() { DirectoryPath = path.FolderPath });
            }
          });

        return this.mSelectDirectoryCommand;
      }
    }

    /// <summary>
    /// Get/set filesystem path to Desktop folder.
    /// </summary>
    public Environment.SpecialFolder DesktopFolder
    {
      get
      {
        return this.mDesktopFolder;
      }
      set
      {
        if (this.mDesktopFolder != value)
        {
          this.mDesktopFolder = value;

          this.RaisePropertyChanged(() => this.DesktopFolder);
          this.OnSelectedFolderChanged();
        }
      }
    }

    /// <summary>
    /// Get/set filesystem path to program files folder.
    /// </summary>
    public Environment.SpecialFolder ProgramFilesFolder
    {
      get
      {
        return this.mProgramFilesFolder;
      }

      set
      {
        if (this.mProgramFilesFolder != value)
        {
          this.mProgramFilesFolder = value;

          this.RaisePropertyChanged(() => this.ProgramFilesFolder);
          this.OnSelectedFolderChanged();
        }
      }
    }

    /// <summary>
    /// Get/set filesystem path to the My Documents folder.
    /// </summary>
    public Environment.SpecialFolder MyDocumentsFolder
    {
      get
      {
        return this.mMyDocumentsFolder;
      }
      set
      {
        if (this.mMyDocumentsFolder != value)
        {
          this.mMyDocumentsFolder = value;

          this.RaisePropertyChanged(() => this.MyDocumentsFolder);
          this.OnSelectedFolderChanged();
        }
      }
    }

    /// <summary>
    /// Get/set filesystem path to My Pictures files folder.
    /// </summary>
    public Environment.SpecialFolder MyPicturesFolder
    {
      get
      {
        return this.mMyPicturesFolder;
      }
      set
      {
        if (this.mMyPicturesFolder != value)
        {
          this.mMyPicturesFolder = value;

          this.RaisePropertyChanged(() => this.MyPicturesFolder);
          this.OnSelectedFolderChanged();
        }
      }
    }
    #endregion properties

    #region methods
    private void OnSelectedFolderChanged()
    {
      if (!mExpanding)
      {
        try
        {
          this.mExpanding = true;
          FolderViewModel child = this.Expand(Folders, SelectedFolder);

          if (child != null)
            child.IsSelected = true;
        }
        finally
        {
          this.mExpanding = false;
        }
      }
    }

    private FolderViewModel Expand(ObservableCollection<FolderViewModel> childFolders, string path)
    {
      if (String.IsNullOrEmpty(path) || childFolders.Count == 0)
      {
        return null;
      }

      string folderName = path;
      if (path.Contains('/') || path.Contains('\\'))
      {
        int idx = path.IndexOfAny(new char[] { '/', '\\' });
        folderName = path.Substring(0, idx);
        path = path.Substring(idx + 1);
      }
      else
      {
        path = null;
      }

      // bugfix: Folder names on windows are case insensitiv
      var results = childFolders.Where<FolderViewModel>(folder => string.Compare(folder.FolderName, folderName, true) == 0);
      if (results != null && results.Count() > 0)
      {
        FolderViewModel fvm = results.First();
        fvm.IsExpanded = true;

        var retVal = Expand(fvm.Folders, path);
        if (retVal != null)
        {
          return retVal;
        }
        else
        {
          return fvm;
        }
      }

      return null;
    }
    #endregion methods
  }
}
