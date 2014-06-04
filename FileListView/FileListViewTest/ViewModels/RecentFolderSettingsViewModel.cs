namespace FileListViewTest.ViewModels
{
  using System.Windows.Input;
  using FileListView.ViewModels.Interfaces;
  using FileListViewTest.Command;
  using FolderBrowser.ViewModels;

  /// <summary>
  /// Class implements commands to manipulate the RecentFolder
  /// list of a particular <seealso cref="IFolderListViewModel"/>.
  /// </summary>
  public class RecentFolderSettingsViewModel : FileListViewTest.ViewModels.Base.ViewModelBase
  {
    #region fields
    private RelayCommand<object> mAddRecentFolder;
    private RelayCommand<object> mRemoveRecentFolder;

    private IFolderListViewModel mFolderView = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="folderView"></param>
    public RecentFolderSettingsViewModel(IFolderListViewModel folderView)
    {
      this.mFolderView = folderView;
    }
    #endregion constructor

    #region properties
    #region Commands
    /// <summary>
    /// Add a folder to the list of recent folders.
    /// </summary>
    public ICommand AddRecentFolder
    {
      get
      {
        if (this.mAddRecentFolder == null)
          this.mAddRecentFolder = new RelayCommand<object>((p) => this.AddRecentFolder_Executed(p));

        return this.mAddRecentFolder;
      }
    }

    /// <summary>
    /// Remove a folder from the list of recent folders.
    /// </summary>
    public ICommand RemoveRecentFolder
    {
      get
      {
        if (this.mRemoveRecentFolder == null)
          this.mRemoveRecentFolder = new RelayCommand<object>(
               (p) => this.RemoveRecentFolder_Executed(p),
               (p) => this.mFolderView.FolderTextPath.SelectedRecentLocation != null);

        return this.mRemoveRecentFolder;
      }
    }
    #endregion Commands
    #endregion properties

    #region methods
    private void AddRecentFolder_Executed(object p)
    {
      string path = p as string;

      var dlg = new FolderBrowser.Views.FolderBrowserDialog();

      var dlgViewModel = new FolderBrowser.ViewModels.DialogViewModel(new BrowserViewModel());
      path = (string.IsNullOrEmpty(path) == true ? @"C:\" : path);
      dlgViewModel.TreeBrowser.SetSelectedFolder(path);

      dlg.DataContext = dlgViewModel;

      bool? bResult = dlg.ShowDialog();

      if (dlgViewModel.DialogCloseResult == true || bResult == true)
        this.mFolderView.AddRecentFolder(dlgViewModel.TreeBrowser.SelectedFolder);
    }

    private void RemoveRecentFolder_Executed(object p)
    {
      string path = p as string;

      this.mFolderView.RemoveRecentFolder(path);
    }
    #endregion methods
  }
}
