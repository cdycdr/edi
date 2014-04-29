namespace FileListViewTest
{
  using System.Windows.Input;
  using FileListView.ViewModels;
  using FolderBrowser.Command;

  /// <summary>
  /// Class implements ...
  /// </summary>
  public class ApplicationViewModel : FileListViewTest.ViewModels.Base.ViewModelBase
  {
    #region fields
    private RelayCommand<object> mAddRecentFolder;
    private RelayCommand<object> mRemoveRecentFolder;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public ApplicationViewModel()
    {
      this.FolderView = new FolderListViewModel();

      this.FolderView.AddRecentFolder( @"C:\temp\");
      this.FolderView.AddRecentFolder( @"C:\windows\test.txt");
      
      this.FolderView.AddFilter("*.exe");
      this.FolderView.AddFilter("*.png|*.jpg|*.jpeg");
      this.FolderView.AddFilter("*.tex");
      this.FolderView.AddFilter("*.txt");
      this.FolderView.AddFilter("*.*");
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Expose a viewmodel that controls the combobox folder drop down
    /// and the fodler/file list view.
    /// </summary>
    public FolderListViewModel FolderView { get; set; }

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
               (p) => this.FolderView.FolderTextPath.SelectedRecentLocation != null);

        return this.mRemoveRecentFolder;
      }
    }
    #endregion properties

    #region methods
    private void AddRecentFolder_Executed(object p)
    {
      string path = p as string;

      FolderBrowser.FolderBrowserDialog dlg = new FolderBrowser.FolderBrowserDialog();

      var dlgViewModel = new FolderBrowser.ViewModel.DialogViewModel();
      path = (string.IsNullOrEmpty(path) == true ? @"C:\" : path);
      dlgViewModel.TreeBrowser.SelectedFolder = path;

      dlg.DataContext = dlgViewModel;

      bool? bResult = dlg.ShowDialog();

      if (dlgViewModel.DialogCloseResult == true || bResult == true)
        this.FolderView.AddRecentFolder(dlgViewModel.TreeBrowser.SelectedFolder);
    }

    private void RemoveRecentFolder_Executed(object p)
    {
      string path = p as string;

      this.FolderView.RemoveRecentFolder(path);
    }
    #endregion methods
  }
}
