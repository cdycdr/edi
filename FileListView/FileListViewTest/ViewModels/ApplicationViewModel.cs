namespace FileListViewTest.ViewModels
{
  using System.Collections.Generic;
  using System.Windows.Input;
  using FileListView.ViewModels;
  using FileListView.ViewModels.Interfaces;
  using FileListViewTest.Command;
  using FolderBrowser.ViewModels;
  using FolderBrowser.ViewModels.Interfaces;

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
      
      this.FolderView.AddFilter("Executeable files", "*.exe;*.bat");
      this.FolderView.AddFilter("Image files", "*.png;*.jpg;*.jpeg");
      this.FolderView.AddFilter("LaTex files", "*.tex");
      this.FolderView.AddFilter("Text files", "*.txt");
      this.FolderView.AddFilter("All Files", "*.*");

      // Construct synchronized folder and tree browser views
      this.SynchronizedFolderView = new FolderListViewModel();

      this.SynchronizedFolderView.AddRecentFolder(@"C:\temp\");
      this.SynchronizedFolderView.AddRecentFolder(@"C:\windows\test.txt");

      this.SynchronizedFolderView.AddFilter("Executeable files", "*.exe;*.bat");
      this.SynchronizedFolderView.AddFilter("Image files", "*.png;*.jpg;*.jpeg");
      this.SynchronizedFolderView.AddFilter("LaTex files", "*.tex");
      this.SynchronizedFolderView.AddFilter("Text files", "*.txt");
      this.SynchronizedFolderView.AddFilter("All Files", "*.*");

      this.SynchronizedTreeBrowser = new BrowserViewModel();
      this.SynchronizedTreeBrowser.SetSpecialFoldersVisibility(false);

      this.SynchronizedFolderView.AttachFolderBrowser(this.SynchronizedTreeBrowser);
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Expose a viewmodel that controls the combobox folder drop down
    /// and the fodler/file list view.
    /// </summary>
    public IFolderListViewModel FolderView { get; set; }

    public IFolderListViewModel SynchronizedFolderView { get; set; }

    /// <summary>
    /// Gets the viewmodel that drives the folder picker control.
    /// </summary>
    public IBrowserViewModel SynchronizedTreeBrowser { get; private set; }

    #region Commands for test case without folderBrowser
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
    #endregion Commands for test case without folderBrowser
    #endregion properties

    #region methods
    /// <summary>
    /// Free resources (if any) when application exits.
    /// </summary>
    internal void ApplicationClosed()
    {
      if (this.FolderView != null)
      {
        this.FolderView.DetachFolderBrowser();
      }
    }

    private void AddRecentFolder_Executed(object p)
    {
      string path;
      FolderListViewModel vm;

      this.ResolveParameterList(p, out path, out vm);

      if (vm == null)
        return;

      var dlg = new FolderBrowser.Views.FolderBrowserDialog();

      var dlgViewModel = new FolderBrowser.ViewModels.DialogViewModel(new BrowserViewModel());
      path = (string.IsNullOrEmpty(path) == true ? @"C:\" : path);
      dlgViewModel.TreeBrowser.SetSelectedFolder( path);

      dlg.DataContext = dlgViewModel;

      bool? bResult = dlg.ShowDialog();

      if (dlgViewModel.DialogCloseResult == true || bResult == true)
        vm.AddRecentFolder(dlgViewModel.TreeBrowser.SelectedFolder);
    }

    private void RemoveRecentFolder_Executed(object p)
    {
      string path;
      FolderListViewModel vm;

      this.ResolveParameterList(p, out path, out vm);

      if (vm == null || path == null)
        return;

      vm.RemoveRecentFolder(path);
    }

    /// <summary>
    /// Resolves the parameterlist retrieved from a multibinding command parameter
    /// which has packed parameters via List<object> container into 1 object.
    /// </summary>
    /// <param name="p"></param>
    /// <param name="path"></param>
    /// <param name="vm"></param>
    private void ResolveParameterList(object p,
                                      out string path, out FolderListViewModel vm)
    {
      path = null;
      vm = null;

      var l = p as List<object>;

      if (l == null)
        return;

      if (l.Count < 2)
        return;

      foreach (var item in l)
      {
        if (item is string)
          path = item as string;
        else
          if (item is FolderListViewModel)
            vm = item as FolderListViewModel;
      }
    }
    #endregion methods
  }
}
