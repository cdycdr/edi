namespace FileListView.ViewModels
{
  using System;
  using System.Linq;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Windows.Input;
  using FileListView.Command;
  using FileSystemModels.Events;
  using FileSystemModels.Models;

  /// <summary>
  /// Class implements a viewmodel that can be used for a
  /// combobox that can be used to browse to different folder locations.
  /// </summary>
  public class FolderComboBoxViewModel : Base.ViewModelBase
  {
    #region fields
    private string mCurrentFolder = string.Empty;
    private FSItemVM mSelectedItem = null;

    private RelayCommand<object> mSelectionChanged = null;
    private string mSelectedRecentLocation = string.Empty;

    private object mLockObject = new object();
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public FolderComboBoxViewModel()
    {
      this.CurrentItems = new ObservableCollection<FSItemVM>();
      this.RecentLocations = new ObservableCollection<string>();
    }
    #endregion constructor

    #region Events
    /// <summary>
    /// Event is fired whenever the path in the text portion of the combobox is changed.
    /// </summary>
    public event EventHandler<FolderChangedEventArgs> OnCurrentPathChanged;
    #endregion

    #region properties
    public ObservableCollection<FSItemVM> CurrentItems { get; set; }

    public FSItemVM SelectedItem
    {
      get
      {
        return this.mSelectedItem;
      }

      set
      {
        if (this.mSelectedItem != value)
        {
          this.mSelectedItem = value;
          this.NotifyPropertyChanged(() => this.SelectedItem);
        }
      }
    }

    #region RecentLocation properties
    public ObservableCollection<string> RecentLocations { get; private set; }

    /// <summary>
    /// Gets/set the selected item of the RecentLocations property.
    /// 
    /// This should be bound by the view (ItemsControl) to find the SelectedItem here.
    /// </summary>
    public string SelectedRecentLocation
    {
      get
      {
        return this.mSelectedRecentLocation;
      }

      set
      {
        if (this.mSelectedRecentLocation != value)
        {
          this.mSelectedRecentLocation = value;
          this.NotifyPropertyChanged(() => this.SelectedRecentLocation);
        }
      }
    }
    #endregion RecentLocation properties

    /// <summary>
    /// Get/sets viewmodel data pointing at the path
    /// of the currently selected folder.
    /// </summary>
    public string CurrentFolder
    {
      get
      {
        return this.mCurrentFolder;
      }

      set
      {
        if (this.mCurrentFolder != value)
        {
          this.mCurrentFolder = value;
          this.NotifyPropertyChanged(() => this.CurrentFolder);
          this.NotifyPropertyChanged(() => this.CurrentFolderToolTip);
        }
      }
    }

    public string CurrentFolderToolTip
    {
      get
      {
        if (string.IsNullOrEmpty(this.mCurrentFolder) == false)
          return string.Format("{0}\n{1}", this.mCurrentFolder,
                                           Local.Strings.SelectLocationCommand_TT);
        else
          return Local.Strings.SelectLocationCommand_TT;
      }
    }
    
    #region commands
    /// <summary>
    /// Command is invoked when the combobox view tells the viewmodel
    /// that the current path selection has changed (via selection changed
    /// event or keyup events).
    /// 
    /// The parameter <paramref name="p"/> can be an array of objects
    /// containing objects of the <seealso cref="FSItemVM"/> type or
    /// p can also be string.
    /// 
    /// Each parameter item that adheres to the above types results in
    /// a OnCurrentPathChanged event being fired with the folder path
    /// as parameter.
    /// </summary>
    /// <param name="p"></param>
    public ICommand SelectionChanged
    {
      get
      {
        if (this.mSelectionChanged == null)
          this.mSelectionChanged = new RelayCommand<object>((p) => this.SelectionChanged_Executed(p));

        return this.mSelectionChanged;
      }
    }
    #endregion commands
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
      if((folderPath = PathModel.ExtractDirectoryRoot(folderPath)) == null)
        return;

      // select this path if its already there
      var results = this.RecentLocations.Where<string>(folder => string.Compare(folder, folderPath, true) == 0);

      // Do not add this twice
      if (results != null)
      {
        if (results.Count() != 0)
          return;
      }

      this.RecentLocations.Add(folderPath);
      this.PopulateView();
    }

    /// <summary>
    /// Remove a recent folder location from the collection of recent folders.
    /// This collection can then be used in the folder combobox drop down
    /// list to store user specific customized folder short-cuts.
    /// </summary>
    /// <param name="folderPath"></param>
    public void RemoveRecentFolder(PathModel folderPath)
    {
      if (folderPath == null)
        return;

      this.RecentLocations.Remove(folderPath.Path);
      this.PopulateView();
    }

    /// <summary>
    /// Can be invoked to refresh the currently visible set of data.
    /// </summary>
    public void PopulateView()
    {
      lock(this.mLockObject)
      {
        ////CurrentItems.Clear();
        string bak = this.CurrentFolder;

        this.CurrentItems.Clear();
        this.CurrentFolder = bak;

        // add special folders
        this.CurrentItems.Add(new FSItemVM(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
                                          FSItemType.Folder, Local.Strings.FolderItem_Desktop));

        this.CurrentItems.Add(new FSItemVM(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                                            FSItemType.Folder, Local.Strings.FolderItem_MyDocuments));

        // Add a seperator (this are templated via x:Null in ControlTemplate)
        this.CurrentItems.Add(null);

        // add drives
        string pathroot = string.Empty;

        if (string.IsNullOrEmpty(this.CurrentFolder) == false)
        {
          try
          {
            pathroot = System.IO.Path.GetPathRoot(this.CurrentFolder);
          }
          catch
          {
            pathroot = string.Empty;
          }
        }

        foreach (string s in Directory.GetLogicalDrives())
        {
          FSItemVM info = new FSItemVM(s, FSItemType.Folder, s);
          this.CurrentItems.Add(info);

          // add items under current folder
          if (string.Compare(pathroot, s, true) == 0)
          {
            string[] dirs = this.CurrentFolder.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 1; i < dirs.Length; i++)
            {
              string curdir = string.Join(string.Empty + System.IO.Path.DirectorySeparatorChar, dirs, 0, i + 1);

              info = new FSItemVM(curdir, FSItemType.Folder, dirs[i], i * 10);

              this.CurrentItems.Add(info);
            }

            // currently selected path was expanded in last for loop -> select the last expanded element 
            if (this.SelectedItem == null)
            {
              this.SelectedItem = this.CurrentItems[CurrentItems.Count - 1];
            
              if (this.OnCurrentPathChanged != null)
                this.OnCurrentPathChanged(this, new FolderChangedEventArgs(this.SelectedItem.GetModel));
            }
          }
        }

        // Force a selection on to the control when there is no selected item, yet
        if (this.CurrentItems != null && this.SelectedItem == null)
        {
          if (this.CurrentItems.Count > 0)
          {
            this.CurrentFolder = this.CurrentItems[0].FullPath;
            this.SelectedItem = this.CurrentItems[0];

            if (this.OnCurrentPathChanged != null)
              this.OnCurrentPathChanged(this, new FolderChangedEventArgs(this.SelectedItem.GetModel));
          }
        }

        // Add a seperator (these are templated via x:Null in ControlTemplate)
        this.CurrentItems.Add(null);

        // add recent locations
        if (this.RecentLocations != null)
        {
          ////List<string> fullpaths = new List<string>();    // remember paths to avoid adding duplicates
          foreach (object o in this.RecentLocations)
          {
            string s = o.ToString();
            s = System.IO.Path.GetDirectoryName(s);

            ////if (!fullpaths.Contains(s))
            ////{
            ////  fullpaths.Add(s);
              string displayName = string.Empty;

              try
              {
                displayName = System.IO.Path.GetFileName(s);
              }
              catch
              {
              }

              if (displayName.Trim() == string.Empty)
                displayName = s;

              FSItemVM it = new FSItemVM(s, FSItemType.Folder, displayName, true);
              ////{
              ////  FullPath = s,
              ////  ShowToolTip = true,
              ////  DisplayName = System.IO.Path.GetFileName(s),
              ////  Type = FSItemType.Folder,
              ////  DisplayIcon = IconExtractor.GetFolderIcon(s).ToImageSource()
              ////};
          
              ////if (it.DisplayName.Trim() == string.Empty)
              ////  it.DisplayName = it.FullPath;
              ////
              this.CurrentItems.Add(it);
            ////}
          }
        }
      }
    }

    /// <summary>
    /// Method executes when the SelectionChanged command is invoked.
    /// The parameter <paramref name="p"/> can be an array of objects
    /// containing objects of the <seealso cref="FSItemVM"/> type or
    /// p can also be string.
    /// 
    /// Each parameter item that adheres to the above types results in
    /// a OnCurrentPathChanged event being fired with the folder path
    /// as parameter.
    /// </summary>
    /// <param name="p"></param>
    private void SelectionChanged_Executed(object p)
    {
      if (p == null)
        return;

      // Check if the given parameter is an array of objects and process it...
      object[] paramObjects = p as object[];
      if (paramObjects != null)
      {
        for (int i = 0; i < paramObjects.Length; i++)
        {
          var item = paramObjects[i] as FSItemVM;

          if (item != null)
          {
            if (item.DirectoryPathExists() == true)
            {
              if (this.OnCurrentPathChanged != null)
                this.OnCurrentPathChanged(this, new FolderChangedEventArgs(item.GetModel));
            }
          }
        }
      }

      // Check if the given parameter is a string, fire a corresponding event if so...
      var paramString = p as string;
      if (paramString != null)
      {
        var path = new PathModel(paramString, FSItemType.Folder);

        if (path.DirectoryPathExists() == true)
        {
          if (this.OnCurrentPathChanged != null)
            this.OnCurrentPathChanged(this, new FolderChangedEventArgs(path));
        }
      }
    }

    internal void ClearRecentFolderCollection()
    {
      if (this.RecentLocations != null)
        this.RecentLocations.Clear();
    }
    #endregion methods
  }
}
