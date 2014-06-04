namespace FileListView.ViewModels
{
  using System.Linq;
  using System.Windows;
  using FileListView.ViewModels.Interfaces;
  using FileSystemModels.Interfaces;
  using FileSystemModels.Models;
  using FolderBrowser.ViewModels;
  using FolderBrowser.ViewModels.Interfaces;

  /// <summary>
  /// Class implements a folder/file view model class
  /// that can be used to dispaly filesystem related content in an <see cref="ItemsControl"/>.
  /// </summary>
  public class FolderListViewModel : Base.ViewModelBase, IFolderListViewModel
  {
    #region fields
    private string mSelectedFolder = string.Empty;

    private object lockObject = new object();
    #endregion fields

    #region constructor
    /// <summary>
    /// Custom class constructor
    /// </summary>
    /// <param name="OnFileOpenMethod"></param>
    public FolderListViewModel(System.EventHandler<FileSystemModels.Events.FileOpenEventArgs> OnFileOpenMethod)
      : this()
    {
      // Remove the standard constructor event that is fired when a user opens a file
      this.FolderItemsView.OnFileOpen -= FolderItemsView_OnFileOpen;

      // ...and establish a new link (if any)
      if (OnFileOpenMethod != null)
        this.FolderItemsView.OnFileOpen += OnFileOpenMethod;
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public FolderListViewModel()
    {
      // This viewmodel can work with or without folderbrowser
      this.FolderBrowser = null;

      this.FolderItemsView = new FileListViewViewModel(new BrowseNavigation());

      this.FolderTextPath = new FolderComboBoxViewModel();
      
      this.Filters = new FilterComboBoxViewModel();
      this.Filters.OnFilterChanged += FileViewFilter_Changed;

      // This is fired when the text path in the combobox changes to another existing folder
      this.FolderTextPath.OnCurrentPathChanged += FolderTextPath_OnCurrentPathChanged;

      // This is fired when the current folder in the listview changes to another existing folder
      this.FolderItemsView.OnCurrentPathChanged += FolderItemsView_OnCurrentPathChanged;

      this.FolderItemsView.RequestEditRecentFolder += FolderItemsView_RequestEditRecentFolder;

      // This event is fired when a user opens a file
      this.FolderItemsView.OnFileOpen += FolderItemsView_OnFileOpen;

      this.FolderTextPath.PopulateView();
    }

    /// <summary>
    /// The list view of folders and files requests to add or remove a folder
    /// to/from the collection of recent folders.
    /// -> Forward event to <seealso cref="FolderComboBoxViewModel"/> who manages
    /// the actual list of recent folders.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void FolderItemsView_RequestEditRecentFolder(object sender, FileSystemModels.Events.RecentFolderEvent e)
    {
      switch (e.Action)
      {
        case FileSystemModels.Events.RecentFolderEvent.RecentFolderAction.Remove:
          this.FolderTextPath.RemoveRecentFolder(e.Folder);
          break;

        case FileSystemModels.Events.RecentFolderEvent.RecentFolderAction.Add:
          this.FolderTextPath.AddRecentFolder(e.Folder.Path);
          break;

        default:
          break;
      }
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Expose a viewmodel that can represent a Folder-ComboBox drop down
    /// element similar to a web browser Uri drop down control but using
    /// local paths only.
    /// </summary>
    public FolderComboBoxViewModel FolderTextPath { get; private set; }

    /// <summary>
    /// Expose a viewmodel that can represent a Filter-ComboBox drop down
    /// similar to the top-right filter/search combo box in Windows Exploer windows.
    /// </summary>
    public FilterComboBoxViewModel Filters { get; private set; }

    /// <summary>
    /// Expose a viewmodel that can support a listview showing folders and files
    /// with their system specific icon.
    /// </summary>
    public IFileListViewModel FolderItemsView { get; private set; }

    /// <summary>
    /// Gets a synchronized <seealso cref="IBrowserViewModel"/> if there is 1 attached
    /// and otherwise null;
    /// </summary>
    public IBrowserViewModel FolderBrowser { get; private set; }

    /// <summary>
    /// Gets the currently selected folder path string.
    /// </summary>
    public string SelectedFolder
    {
      get
      {
        return this.mSelectedFolder;
      }

      private set
      {
        if (this.mSelectedFolder != value)
        {
          this.mSelectedFolder = value;
          this.NotifyPropertyChanged(() => this.SelectedFolder);
        }
      }
    }
    #endregion properties

    #region methods
    #region Folder Browser Attachment methods
    /// <summary>
    /// Attach a <seealso cref="IBrowserViewModel"/> to synchronize the
    /// current path with the <seealso cref="IFolderListViewModel"/>.
    /// </summary>
    /// <param name="folderBrowser"></param>
    public void AttachFolderBrowser(IBrowserViewModel folderBrowser)
    {
      this.DetachFolderBrowser();

      this.FolderBrowser = folderBrowser;

      if (this.FolderBrowser != null)
      {
        this.FolderBrowser.FolderSelectionChangedEvent += FolderBrowser_FolderSelectionChangedEvent;
        this.FolderBrowser.RequestEditRecentFolder += FolderItemsView_RequestEditRecentFolder;

        this.FolderBrowser.SetSelectedFolder(this.SelectedFolder);
      }
    }

    /// <summary>
    /// Detach the <seealso cref="IBrowserViewModel"/> (if any) to stop
    /// synchronizing the current path with the <seealso cref="IFolderListViewModel"/>.
    /// elements with it.
    /// </summary>
    public void DetachFolderBrowser()
    {
      if (this.FolderBrowser != null)
      {
        this.FolderBrowser.FolderSelectionChangedEvent -= FolderBrowser_FolderSelectionChangedEvent;
        this.FolderBrowser.RequestEditRecentFolder -= FolderItemsView_RequestEditRecentFolder;
      }

      this.FolderBrowser = null;
    }
    #endregion Folder Browser Attachment methods

    #region Explorer settings model
    /// <summary>
    /// Configure this viewmodel (+ attached browser viewmodel) with the given settings.
    /// </summary>
    /// <param name="settings"></param>
    /// <returns></returns>
    bool IExplorerSettings.ConfigureExplorerSettings(ExplorerSettingsModel settings)
    {
      if (settings == null)
        return false;

      try
      {
        // Set currently view folder in Explorer Tool Window
        this.ConfigureCurrentFolder(settings.UserProfile.CurrentPath.Path);

        this.Filters.ClearFilter();

        // Set file filter in file/folder list view
        foreach (var item in settings.FilterCollection)
          this.Filters.AddFilter(item.FilterDisplayName, item.FilterText);

        // Add a current filter setting (if any is available)
        if (settings.UserProfile.CurrentFilter != null)
        {
          this.Filters.SetCurrentFilter(settings.UserProfile.CurrentFilter.FilterDisplayName,
                                        settings.UserProfile.CurrentFilter.FilterText);
        }

        this.FolderTextPath.ClearRecentFolderCollection();

        // Set collection of recent folder locations
        foreach (var item in settings.RecentFolders)
          this.FolderTextPath.AddRecentFolder(item);

        this.FolderItemsView.ShowIcons = settings.ShowIcons;
        this.FolderItemsView.SetIsFolderVisible(settings.ShowFolders);
        this.FolderItemsView.ShowHidden = settings.ShowHiddenFiles;

        if (this.FolderBrowser != null)
        {
          this.FolderBrowser.SpecialFolders.Clear();

          foreach (var item in settings.SpecialFolders)
            this.FolderBrowser.SpecialFolders.Add(new CustomFolderItemViewModel(item.SpecialFolder));

          this.FolderBrowser.SetSpecialFoldersVisibility(settings.ShowSpecialFoldersInTreeBrowser);
        }
      }
      catch
      {
        return false;
      }

      return true;
    }

    /// <summary>
    /// Compare given <paramref name="input"/> settings with current settings
    /// and return a new settings model if the current settings
    /// changed in comparison to the given <paramref name="input"/> settings.
    /// 
    /// Always return current settings if <paramref name="input"/> settings is null.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    ExplorerSettingsModel IExplorerSettings.GetExplorerSettings(ExplorerSettingsModel input)
    {
      var settings = new ExplorerSettingsModel();

      try
      {
        settings.UserProfile.SetCurrentPath(this.SelectedFolder);

        foreach (var item in settings.RecentFolders)
          this.AddRecentFolder(item);

        foreach (var item in settings.FilterCollection)
        {
          if (item == settings.UserProfile.CurrentFilter)
            this.AddFilter(item.FilterDisplayName, item.FilterText, true);
          else
            this.AddFilter(item.FilterDisplayName, item.FilterText);
        }

        foreach (var item in this.Filters.CurrentItems)
        {
          if (item == this.Filters.SelectedItem)
            settings.AddFilter(item.FilterDisplayName, item.FilterText, true);
          else
            settings.AddFilter(item.FilterDisplayName, item.FilterText);
        }

        foreach (var item in this.FolderTextPath.RecentLocations)
          settings.AddRecentFolder(item);

        settings.ShowIcons = this.FolderItemsView.ShowIcons;
        settings.ShowFolders = this.FolderItemsView.ShowFolders;
        settings.ShowHiddenFiles = this.FolderItemsView.ShowHidden;

        if (this.FolderBrowser != null)
        {
          foreach (var item in this.FolderBrowser.SpecialFolders)
            settings.AddSpecialFolder(item.SpecialFolder);

          settings.ShowSpecialFoldersInTreeBrowser = this.FolderBrowser.IsSpecialFoldersVisisble;
        }

        if (ExplorerSettingsModel.CompareSettings(input, settings) == false)
          return settings;
        else
          return null;
      }
      catch
      {
        throw;
      }

      return settings;
    }
    #endregion Explorer settings model

    #region Recent Folder Methods
    /// <summary>
    /// Add a recent folder location into the collection of recent folders.
    /// This collection can then be used in the folder combobox drop down
    /// list to store user specific customized folder short-cuts.
    /// </summary>
    /// <param name="folderPath"></param>
    public void AddRecentFolder(string folderPath)
    {
      this.FolderTextPath.AddRecentFolder(folderPath);
    }

    /// <summary>
    /// Removes a recent folder location into the collection of recent folders.
    /// This collection can then be used in the folder combobox drop down
    /// list to store user specific customized folder short-cuts.
    /// </summary>
    /// <param name="path"></param>
    public void RemoveRecentFolder(string path)
    {
      if (string.IsNullOrEmpty(path) == true)
        return;

      this.FolderTextPath.RecentLocations.Remove(path);
      this.FolderTextPath.PopulateView();
    }
    #endregion Recent Folder Methods

    #region Change filter methods
    /// <summary>
    /// Add a new filter argument to the list of filters and
    /// select this filter if <paramref name="bSelectNewFilter"/>
    /// indicates it.
    /// </summary>
    /// <param name="filterString"></param>
    /// <param name="bSelectNewFilter"></param>
    public void AddFilter(string filterString,
                          bool bSelectNewFilter = false)
    {
      this.Filters.AddFilter(filterString, bSelectNewFilter);
    }

    /// <summary>
    /// Add a new filter argument to the list of filters and
    /// select this filter if <paramref name="bSelectNewFilter"/>
    /// indicates it.
    /// </summary>
    /// <param name="name"></param>
    /// <param name="filterString"></param>
    /// <param name="bSelectNewFilter"></param>
    public void AddFilter(string name, string filterString,
                          bool bSelectNewFilter = false)
    {
      this.Filters.AddFilter(name, filterString, bSelectNewFilter);
    }
    #endregion Change filter methods

    /// <summary>
    /// Navigates to the folder indicated by <paramref name="sFolder"/>
    /// and updates all related viewmodels.
    /// </summary>
    /// <param name="sFolder"></param>
    public void NavigateToFolder(string sFolder)
    {
      this.SelectedFolder = sFolder;
      this.FolderItemsView.NavigateToThisFolder(sFolder);

      this.FolderTextPath.CurrentFolder = sFolder;
      this.FolderTextPath.PopulateView();
    }
    
    /// <summary>
    /// Executes when the file open event is fired and class was constructed with statndard constructor.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void FolderItemsView_OnFileOpen(object sender,
                                              FileSystemModels.Events.FileOpenEventArgs e)
    {
      MessageBox.Show("File Open:" + e.FileName);
    }

    /// <summary>
    /// Method executes when the text path in the combobox changes to another existing folder.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FolderTextPath_OnCurrentPathChanged(object sender,
                                                     FileSystemModels.Events.FolderChangedEventArgs e)
    {
      lock(this.lockObject)
      {
        if (string.Compare(this.SelectedFolder, e.Folder.Path, true) != 0)
        {
          this.SelectedFolder = e.Folder.Path;

          if (e.Folder.Path != this.FolderItemsView.CurrentFolder && sender != this)
            this.FolderItemsView.UpdateView(e.Folder.Path);
 
          if (this.FolderBrowser != null)
          {
            if (sender != this.FolderBrowser)
              this.FolderBrowser.SetSelectedFolder(this.SelectedFolder);
          }
        }
      }
    }

    /// <summary>
    /// Applies the file/directory filter from the combobox on the listview entries.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FileViewFilter_Changed(object sender, FileSystemModels.Events.FilterChangedEventArgs e)
    {
      FolderItemsView.ApplyFilter(e.FilterText);
    }

    /// <summary>
    /// Method executes when the current folder in the listview changes to another existing folder
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FolderItemsView_OnCurrentPathChanged(object sender,
                                                      FileSystemModels.Events.FolderChangedEventArgs e)
    {
      lock(this.lockObject)
      {
        if (string.Compare(this.FolderTextPath.CurrentFolder, e.Folder.Path, true) != 0)
        {
          this.FolderTextPath.CurrentFolder = e.Folder.Path;
          this.FolderTextPath.PopulateView();
  
          if (this.FolderBrowser != null)
          {
            if (sender != this.FolderBrowser)
              this.FolderBrowser.SetSelectedFolder(this.SelectedFolder);
          }
        }
      }
    }

    /// <summary>
    /// Method executes when the currently selected path is
    /// changed in the attached folder browser viewmodel.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FolderBrowser_FolderSelectionChangedEvent(object sender, FileSystemModels.Events.FolderChangedEventArgs e)
    {
      if (e == null)
        return;

      if (e.Folder.DirectoryPathExists() == false)
        return;

      if (sender != this.FolderItemsView)
        this.FolderItemsView_OnCurrentPathChanged(sender,
                  new FileSystemModels.Events.FolderChangedEventArgs(new PathModel(e.Folder)));

      if (sender != this.FolderTextPath)
        this.FolderTextPath_OnCurrentPathChanged(sender,
                  new FileSystemModels.Events.FolderChangedEventArgs(new PathModel(e.Folder)));
    }


    private void ConfigureCurrentFolder(string path)
    {
      this.SelectedFolder = path;
      this.FolderTextPath.SelectedItem = new FSItemVM(path, FSItemType.Folder, path, true, 0);

      this.FolderBrowser.SetSelectedFolder(this.SelectedFolder);
    }
    #endregion methods
  }
}
