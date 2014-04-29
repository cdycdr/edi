namespace FileListView.ViewModels
{
  using System;
  using System.Collections.ObjectModel;
  using System.Windows;

  /// <summary>
  /// Class implements a folder/file view model class
  /// that can be used to dispaly filesystem related content in an <see cref="ItemsControl"/>.
  /// </summary>
  public class FolderListViewModel : Base.ViewModelBase
  {
    #region fields
    private string mSelectedFolder = string.Empty;
    #endregion fields

    #region constructor
    /// <summary>
    /// Custom class constructor
    /// </summary>
    /// <param name="OnFileOpenMethod"></param>
    public FolderListViewModel(System.EventHandler<Events.FileOpenEventArgs> OnFileOpenMethod)
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
      this.FolderItemsView = new FileListViewViewModel();

      this.FolderTextPath = new FolderComboBoxViewModel();
      
      this.Filters = new FilterComboBoxViewModel();
      this.Filters.OnFilterChanged += FileViewFilter_Changed;

      // This is fired when the text path in the combobox changes to another existing folder
      this.FolderTextPath.OnCurrentPathChanged += FolderTextPath_OnCurrentPathChanged;

      // This is fired when the current folder in the listview changes to another existing folder
      this.FolderItemsView.OnCurrentPathChanged += FolderItemsView_OnCurrentPathChanged;

      // This event is fired when a user opens a file
      this.FolderItemsView.OnFileOpen += FolderItemsView_OnFileOpen;

      this.FolderTextPath.PopulateView();
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Expose a viewmodel that can represent a Folder-ComboBox drop down
    /// element similar to a web browser Uri drop down control but using
    /// local paths only.
    /// </summary>
    public FolderComboBoxViewModel FolderTextPath { get; set; }

    /// <summary>
    /// Expose a viewmodel that can represent a Filter-ComboBox drop down
    /// similar to the top-right filter/search combo box in Windows Exploer windows.
    /// </summary>
    public FilterComboBoxViewModel Filters { get; set; }

    /// <summary>
    /// Expose a viewmodel that can support a listview showing folders and files
    /// with their system specific icon.
    /// </summary>
    public FileListViewViewModel FolderItemsView { get; set; }

    public string SelectedFolder
    {
      get
      {
        return this.mSelectedFolder;
      }

      set
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
    /// <summary>
    /// Add a recent folder location into the collection of recent folders.
    /// This collection can then be used in the folder combobox drop down
    /// list to store user specific customized folder short-cuts.
    /// </summary>
    /// <param name="folderPath"></param>
    public void AddRecentFolder(string folderPath)
    {
      bool bExists = false;
      
      try
      {
        bExists = System.IO.Directory.Exists(folderPath);
      }
      catch
      {
      }

      if (bExists == true)
      {
        this.FolderTextPath.RecentLocations.Add(folderPath);
        this.FolderTextPath.PopulateView();
      }
      else
      {
        bExists = false;
        string path = string.Empty;

        try
        {
          // check if this is a file reference and attempt to get its path
          path = System.IO.Path.GetDirectoryName(folderPath);
          bExists = System.IO.Directory.Exists(path);
        }
        catch
        {
        }     
        
        if (bExists == true)
        {
          path += System.IO.Path.DirectorySeparatorChar;

          this.FolderTextPath.RecentLocations.Add(path);
          this.FolderTextPath.PopulateView();
        }
      }
    }

    public void RemoveRecentFolder(string path)
    {
      if (string.IsNullOrEmpty(path) == true)
        return;

      this.FolderTextPath.RecentLocations.Remove(path);
      this.FolderTextPath.PopulateView();
    }

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
    protected void FolderItemsView_OnFileOpen(object sender, Events.FileOpenEventArgs e)
    {
      MessageBox.Show("File Open:" + e.FileName);
    }

    /// <summary>
    /// Method executes when the text path in the combobox changes to another existing folder.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FolderTextPath_OnCurrentPathChanged(object sender, Events.FolderChangedEventArgs e)
    {
      this.SelectedFolder = e.FilePath;
      this.FolderItemsView.UpdateView(e.FilePath);
    }

    /// <summary>
    /// Applies the file/directory filter from the combobox on the listview entries.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FileViewFilter_Changed(object sender, Events.FilterChangedEventArgs e)
    {
      FolderItemsView.ApplyFilter(e.FilterText);
    }

    /// <summary>
    /// Method executes when the current folder in the listview changes to another existing folder
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FolderItemsView_OnCurrentPathChanged(object sender, Events.FolderChangedEventArgs e)
    {
      this.FolderTextPath.CurrentFolder = e.FilePath;
      this.FolderTextPath.PopulateView();
    }
    #endregion methods
  }
}
