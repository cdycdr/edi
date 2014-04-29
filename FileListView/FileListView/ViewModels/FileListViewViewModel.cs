namespace FileListView.ViewModels
{
  using System;
  using System.Collections;
  using System.Collections.ObjectModel;
  using System.Diagnostics;
  using System.IO;
  using System.Windows.Input;
  using System.Windows.Media;
  using System.Windows.Media.Imaging;
  using FileListView.Command;
  using FileListView.Events;
  using FileListView.Models;

  /// <summary>
  /// Class implements a common ground class for organizing a filter combobox
  /// view with a file list view.
  /// </summary>
  public class FileListViewViewModel : Base.ViewModelBase
  {
    #region fields
    /// <summary>
    /// Defines the delimitor for multiple regular expression filter statements.
    /// eg: "*.txt;*.ini"
    /// </summary>
    private const char FilterSplitCharacter = ';';

    /// <summary>
    /// Determines whether the redo stack (FutureFolders) should be cleared when the CurrentFolder changes next time
    /// </summary>
    private string mFilterString = string.Empty;
    private string[] mParsedFilter = null;

    private bool mShowFolders = true;
    private bool mShowHidden = true;
    private bool mShowIcons = true;

    private RelayCommand<object> mDownCommand = null;

    private RelayCommand<object> mUpCommand = null;
    private RelayCommand<object> mForwardCommand = null;
    private RelayCommand<object> mBackCommand = null;
    private RelayCommand<object> mRefreshCommand = null;
    private RelayCommand<object> mToggleIsFolderVisibleCommand = null;
    private RelayCommand<object> mToggleIsIconVisibleCommand = null;
    private RelayCommand<object> mToggleIsHiddenVisibleCommand = null;

    private RelayCommand<object> mOpenContainingFolderCommand = null;
    private RelayCommand<object> mOpenInWindowsCommand = null;
    private RelayCommand<object> mCopyPathCommand = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public FileListViewViewModel()
    {
      this.CurrentItems = new ObservableCollection<FSItemVM>();
      this.RecentFolders = new Stack<string>();
      this.FutureFolders = new Stack<string>();

      this.mParsedFilter = FileListViewViewModel.GetParsedFilters(this.mFilterString);
    }
    #endregion constructor

    #region Events
    /// <summary>
    /// Event is fired to indicate that user wishes to open a file via this viewmodel.
    /// </summary>
    public event EventHandler<FileOpenEventArgs> OnFileOpen;

    /// <summary>
    /// Event is fired whenever the path in the text portion of the combobox is changed.
    /// </summary>
    public event EventHandler<FolderChangedEventArgs> OnCurrentPathChanged;
    #endregion

    #region properties
    /// <summary>
    /// Gets/sets list of files and folders to be displayed in connected view.
    /// </summary>
    public ObservableCollection<FSItemVM> CurrentItems { get; private set; }

    /// <summary>
    /// Gets/sets whether the list of folders and files should include folders or not.
    /// </summary>
    public bool ShowFolders
    {
      get
      {
        return this.mShowFolders;
      }

      private set
      {
        if (this.mShowFolders != value)
        {
          this.mShowFolders = value;
          this.NotifyPropertyChanged(() => this.ShowFolders);
        }
      }
    }

    /// <summary>
    /// Gets/sets whether the list of folders and files includes hidden folders or files.
    /// </summary>
    public bool ShowHidden
    {
      get
      {
        return this.mShowHidden;
      }

      private set
      {
        if (this.mShowHidden != value)
        {
          this.mShowHidden = value;
          this.NotifyPropertyChanged(() => this.ShowHidden);
        }
      }
    }

    /// <summary>
    /// Gets/sets whether file or directory icons should be shown or not.
    /// </summary>
    public bool ShowIcons
    {
      get
      {
        return this.mShowIcons;
      }

      private set
      {
        if (this.mShowIcons != value)
        {
          this.mShowIcons = value;
          this.NotifyPropertyChanged(() => this.ShowIcons);
        }
      }
    }

    #region commands
    /// <summary>
    /// Gets the command to navigate downwards - to a child folder -
    /// of the currently visited folder.
    /// </summary>
    public ICommand NavigateDownCommand
    {
      get
      {
        if (this.mDownCommand == null)
          this.mDownCommand = new RelayCommand<object>((p) => this.DownCommand_Executed(p));

        return this.mDownCommand;
      }
    }

    /// <summary>
    /// Gets the command to navigate upward - to parent folder -
    /// of the currently visited folder.
    /// </summary>
    public ICommand NavigateUpCommand
    {
      get
      {
        if (this.mUpCommand == null)
          this.mUpCommand = new RelayCommand<object>((p) => this.UpCommand_Executed());

        return this.mUpCommand;
      }
    }

    /// <summary>
    /// Gets the command to navigate forward in the history of visited folders.
    /// </summary>
    public ICommand NavigateForwardCommand
    {
      get
      {
        if (this.mForwardCommand == null)
          this.mForwardCommand = new RelayCommand<object>((p) => this.ForwardCommand_Executed(),
                                                          (p) => this.FutureFolders.Count > 0);

        return this.mForwardCommand;
      }
    }

    /// <summary>
    /// Gets the command to navigate back in the history of visited folders.
    /// </summary>
    public ICommand NavigateBackCommand
    {
      get
      {
        if (this.mBackCommand == null)
          this.mBackCommand = new RelayCommand<object>((p) => this.BackCommand_Executed(),
                                                       (p) => this.RecentFolders.Count > 0);


        return this.mBackCommand;
      }
    }

    /// <summary>
    /// Gets the command that updates the currently viewed
    /// list of directory items (files and sub-directories).
    /// </summary>
    public ICommand RefreshCommand
    {
      get
      {
        if (this.mRefreshCommand == null)
          this.mRefreshCommand = new RelayCommand<object>((p) => this.PopulateView());

        return this.mRefreshCommand;
      }
    }

    /// <summary>
    /// Toggles the visibiliy of folders in the folder/files listview.
    /// </summary>
    public ICommand ToggleIsFolderVisibleCommand
    {
      get
      {
        if (this.mToggleIsFolderVisibleCommand == null)
          this.mToggleIsFolderVisibleCommand = new RelayCommand<object>((p) => this.ToggleIsFolderVisible_Executed());

        return this.mToggleIsFolderVisibleCommand;
      }
    }

    /// <summary>
    /// Toggles the visibiliy of icons in the folder/files listview.
    /// </summary>
    public ICommand ToggleIsIconVisibleCommand
    {
      get
      {
        if (this.mToggleIsIconVisibleCommand == null)
          this.mToggleIsIconVisibleCommand = new RelayCommand<object>((p) => this.ToggleIsIconVisible_Executed());

        return this.mToggleIsIconVisibleCommand;
      }
    }

    /// <summary>
    /// Toggles the visibiliy of hidden files/folders in the folder/files listview.
    /// </summary>
    public ICommand ToggleIsHiddenVisibleCommand
    {
      get
      {
        if (this.mToggleIsHiddenVisibleCommand == null)
          this.mToggleIsHiddenVisibleCommand = new RelayCommand<object>((p) => this.ToggleIsHiddenVisible_Executed());

        return this.mToggleIsHiddenVisibleCommand;
      }
    }

    public ICommand OpenContainingFolderCommand
    {
      get
      {
        if (this.mOpenContainingFolderCommand == null)
          this.mOpenContainingFolderCommand = new RelayCommand<object>(
            (p) => 
            {
              var path = p as FSItemVM;

              if (path == null)
                return;

              if (string.IsNullOrEmpty(path.FullPath) == true)
                return;

              FileListViewViewModel.OpenContainingFolderCommand_Executed(path.FullPath);
            });

        return this.mOpenContainingFolderCommand;
      }
    }

    public ICommand OpenInWindowsCommand
    {
      get
      {
        if (this.mOpenInWindowsCommand == null)
          this.mOpenInWindowsCommand = new RelayCommand<object>(
            (p) =>
            {
              var path = p as FSItemVM;

              if (path == null)
                return;

              if (string.IsNullOrEmpty(path.FullPath) == true)
                return;

              FileListViewViewModel.OpenInWindowsCommand_Executed(path.FullPath);
            });

        return this.mOpenInWindowsCommand;
      }
    }

    public ICommand CopyPathCommand
    {
      get
      {
        if (this.mCopyPathCommand == null)
          this.mCopyPathCommand = new RelayCommand<object>(
            (p) =>
            {
              var path = p as FSItemVM;

              if (path == null)
                return;

              if (string.IsNullOrEmpty(path.FullPath) == true)
                return;

              FileListViewViewModel.CopyPathCommand_Executed(path.FullPath);
            });

        return this.mCopyPathCommand;
      }
    }   
    #endregion commands

    #region privates
    /// <summary>
    /// Gets/sets the current folder which is being
    /// queried to list the current files and folders for display.
    /// </summary>
    private string CurrentFolder { get; set; }

    /// <summary>
    /// Gets/sets the undo stacks for navigation.
    /// </summary>
    private Stack<string> RecentFolders { get; set; }

    /// <summary>
    /// Gets/sets the redo stack for navigation.
    /// </summary>
    private Stack<string> FutureFolders { get; set; }
    #endregion privates
    #endregion properties

    #region methods
    /// <summary>
    /// Fills the CurrentItems property for display in ItemsControl
    /// based view (ListBox, ListView etc.).
    /// 
    /// This method wraps a parameterized version of the same method 
    /// with a call that contains the standard data field.
    /// </summary>
    protected void PopulateView()
    {
      this.PopulateView(this.mParsedFilter);
    }

    /// <summary>
    /// Methid is executed when a listview item is double clicked.
    /// </summary>
    /// <param name="p"></param>
    protected void DownCommand_Executed(object p)
    {
      FSItemVM info = p as FSItemVM;

      if (info != null)
      {
        if (info.Type == FSItemType.Folder)
        {
          this.RecentFolders.Push(this.CurrentFolder);
          this.FutureFolders.Clear();
          this.CurrentFolder = info.FullPath;
          this.PopulateView();

          if (this.OnCurrentPathChanged != null)
            this.OnCurrentPathChanged(this, new FolderChangedEventArgs() { FilePath = info.FullPath });
        }
        else
        {
          if (info.Type == FSItemType.File && this.OnFileOpen != null)
            this.OnFileOpen(this, new FileOpenEventArgs() { FileName = info.FullPath });
        }
      }
    }

    /// <summary>
    /// Navigates towards the root of the current folder.
    /// </summary>
    protected void UpCommand_Executed()
    {
      string[] dirs = this.CurrentFolder.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
      if (dirs.Length > 1)
      {
        string newf = string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), dirs, 0, dirs.Length - 1);

        if (dirs.Length == 2)
          newf += System.IO.Path.DirectorySeparatorChar;

        this.RecentFolders.Push(this.CurrentFolder);
        this.FutureFolders.Clear();
        this.CurrentFolder = newf;

        if (this.OnCurrentPathChanged != null)
          this.OnCurrentPathChanged(this, new FolderChangedEventArgs() { FilePath = this.CurrentFolder });
      }
    }

    /// <summary>
    /// Navigates to a previously visited folder (if any).
    /// </summary>
    protected void BackCommand_Executed()
    {
      if (this.RecentFolders.Count > 0)
      {
        // top of stack is always last valid folder
        this.FutureFolders.Push(this.CurrentFolder);
        this.CurrentFolder = this.RecentFolders.Pop();

        if (this.OnCurrentPathChanged != null)
          this.OnCurrentPathChanged(this, new FolderChangedEventArgs() { FilePath = this.CurrentFolder });
      }
    }

    /// <summary>
    /// Navigates to a folder that was visited before navigating back (if any).
    /// </summary>
    protected void ForwardCommand_Executed()
    {
      if (this.FutureFolders.Count > 0)
      {
        this.RecentFolders.Push(this.CurrentFolder);
        this.CurrentFolder = this.FutureFolders.Pop();

        if (this.OnCurrentPathChanged != null)
          this.OnCurrentPathChanged(this, new FolderChangedEventArgs() { FilePath = this.CurrentFolder });
      }
    }

    internal void NavigateToThisFolder(string sFolder)
    {
      this.RecentFolders.Push(this.CurrentFolder);
      this.UpdateView(sFolder);
    }

    internal void ApplyFilter(string filterText)
    {
      this.mFilterString = filterText;

      string[] tempParsedFilter = FileListViewViewModel.GetParsedFilters(this.mFilterString);

      // Optimize nultiple requests for populating same view with unchanged filter away
      if (tempParsedFilter != this.mParsedFilter)
      {
        this.mParsedFilter = tempParsedFilter;
        this.PopulateView();
      }
    }

    internal void UpdateView(string p)
    {
      if (string.IsNullOrEmpty(p) == true)
        return;

      this.CurrentFolder = p;
      this.PopulateView();
    }

    private static string[] GetParsedFilters(string inputFilterString)
    {
      string[] filterString = { "*" };

      try
      {
        if (string.IsNullOrEmpty(inputFilterString) == false)
        {
          if (inputFilterString.Split(FileListViewViewModel.FilterSplitCharacter).Length > 1)
            filterString = inputFilterString.Split(FileListViewViewModel.FilterSplitCharacter);
          else
          {
            // Add asterix at front and beginning if user is too non-technical to type it.
            if (inputFilterString.Contains("*") == false)
              filterString = new string[] { "*" + inputFilterString + "*" };
            else
              filterString = new string[] { inputFilterString };
          }
        }
      }
      catch
      {
      }

      return filterString;
    }

    /// <summary>
    /// Determine whether a given path is an exeisting directory or not.
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private bool IsPathDirectory(string path)
    {
      if (string.IsNullOrEmpty(path) == true)
        return false;

      bool IsPath = false;

      try
      {
        IsPath = System.IO.Directory.Exists(path);
      }
      catch
      {
      }

      return IsPath;
    }

    /// <summary>
    /// Fills the CurrentItems property for display in ItemsControl
    /// based view (ListBox, ListView etc.)
    /// 
    /// This version is parameterized since the filterstring can be parsed
    /// seperately and does not need to b parsed each time when this method
    /// executes.
    /// </summary>
    private void PopulateView(string[] filterString)
    {
      this.CurrentItems.Clear();

      if (this.IsPathDirectory(this.CurrentFolder) == false)
        return;

      try
      {
        DirectoryInfo cur = new DirectoryInfo(this.CurrentFolder);
        ImageSource dummy = new BitmapImage();

        // Retrieve and add (filtered) list of directories
        if (this.ShowFolders)
        {
          string[] directoryFilter = null;

          if (filterString != null)
            directoryFilter = new ArrayList(filterString).ToArray() as string[];

          directoryFilter = null;

          foreach (DirectoryInfo dir in cur.SelectDirectoriesByFilter(directoryFilter))
          {
            if (dir.Attributes.HasFlag(FileAttributes.Hidden) == true)
            {
              if (this.ShowHidden == false)
              {
                if ((dir.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                  continue;
              }
            }

            FSItemVM info = new FSItemVM()
            {
              FullPath = dir.FullName,
              DisplayName = dir.Name,
              Type = FSItemType.Folder
            };

            if (this.ShowIcons == false)
              info.DisplayIcon = dummy;  // to prevent the icon from being loaded from file later

            this.CurrentItems.Add(info);
          }
        }

        // Retrieve and add (filtered) list of files in current directory
        foreach (FileInfo f in cur.SelectFilesByFilter(filterString))
        {
          if (this.ShowHidden == false)
          {
            if (f.Attributes.HasFlag(FileAttributes.Hidden) == true)
            {
              if ((f.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                continue;
            }
          }

          FSItemVM info = new FSItemVM()
          {
            FullPath = f.FullName,
            DisplayName = f.Name,   // System.IO.Path.GetFileName(s),
            Type = FSItemType.File
          };

          if (this.ShowIcons == false)
            info.DisplayIcon = dummy;  // to prevent the icon from being loaded from file later

          this.CurrentItems.Add(info);
        }
      }
      catch
      {
      }

      // reset column width manually (otherwise it is not updated)
      ////this.mFileListView.TheGVColumn.Width = this.mFileListView.TheGVColumn.ActualWidth;
      ////this.mFileListView.TheGVColumn.Width = double.NaN;
    }

    private void ToggleIsFolderVisible_Executed()
    {
      this.ShowFolders = !this.ShowFolders;
      this.PopulateView();
    }

    private void ToggleIsIconVisible_Executed()
    {
      this.ShowIcons = !this.ShowIcons;
      this.PopulateView();
    }

    private void ToggleIsHiddenVisible_Executed()
    {
      this.ShowHidden = !this.ShowHidden;
      this.PopulateView();
    }

    #region FileSystem Commands
    /// <summary>
    /// Convinience method to open Windows Explorer with a selected file (if it exists).
    /// Otherwise, Windows Explorer is opened in the location where the file should be at.
    /// </summary>
    /// <param name="sFileName"></param>
    /// <returns></returns>
    public static bool OpenContainingFolderCommand_Executed(string sFileName)
    {
      if (string.IsNullOrEmpty(sFileName) == true)
        return false;

      try
      {
        if (System.IO.File.Exists(sFileName) == true)
        {
          // combine the arguments together it doesn't matter if there is a space after ','
          string argument = @"/select, " + sFileName;

          System.Diagnostics.Process.Start("explorer.exe", argument);
          return true;
        }
        else
        {
          string sParentDir = string.Empty;

          if (System.IO.Directory.Exists(sFileName) == true)
            sParentDir = sFileName;
          else
            sParentDir = System.IO.Directory.GetParent(sFileName).FullName;

          if (System.IO.Directory.Exists(sParentDir) == false)
          {
            return false;
            ////Msg.Show(string.Format(Local.Strings.STR_MSG_DIRECTORY_DOES_NOT_EXIST, sParentDir),
            ////     Local.Strings.STR_MSG_ERROR_FINDING_RESOURCE,
            ////         MsgBoxButtons.OK, MsgBoxImage.Error);
          }
          else
          {
            // combine the arguments together it doesn't matter if there is a space after ','
            string argument = @"/select, " + sParentDir;

            System.Diagnostics.Process.Start("explorer.exe", argument);

            return true;
          }
        }
      }
      catch (System.Exception ex)
      {
        ////Msg.Show(string.Format("{0}\n'{1}'.", ex.Message, (sFileName == null ? string.Empty : sFileName)),
        ////          Local.Strings.STR_MSG_ERROR_FINDING_RESOURCE,
        ////          MsgBoxButtons.OK, MsgBoxImage.Error);
        return false;
      }
    }

    /// <summary>
    /// Process command when a hyperlink has been clicked.
    /// Start a web browser and let it browse to where this points to...
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void OpenInWindowsCommand_Executed(string sFileName)
    {
      if (string.IsNullOrEmpty(sFileName) == true)
        return;

      try
      {
        Process.Start(new ProcessStartInfo(sFileName));
        ////OpenFileLocationInWindowsExplorer(whLink.NavigateUri.OriginalString);
      }
      catch (System.Exception ex)
      {
        ////Msg.Show(string.Format(CultureInfo.CurrentCulture, "{0}\n'{1}'.", ex.Message, (whLink.NavigateUri == null ? string.Empty : whLink.NavigateUri.ToString())),
        ////         Local.Strings.STR_MSG_ERROR_FINDING_RESOURCE,
        ////         MsgBoxButtons.OK, MsgBoxImage.Error);
      }
    }

    /// <summary>
    /// A hyperlink has been clicked. Start a web browser and let it browse to where this points to...
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void CopyPathCommand_Executed(string sFileName)
    {
      if (string.IsNullOrEmpty(sFileName) == true)
        return;

      try
      {
        System.Windows.Clipboard.SetText(sFileName);
      }
      catch
      {
      }
    }
    #endregion FileSystem Commands
    #endregion methods
  }
}
