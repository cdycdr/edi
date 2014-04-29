namespace FileListView.ViewModels
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Windows.Input;
  using FileListView.Command;
  using FileListView.Events;
  using FileListView.Models;

  /// <summary>
  /// Class implements ...
  /// </summary>
  public class FolderComboBoxViewModel : Base.ViewModelBase
  {
    #region fields
    private string mCurrentFolder = string.Empty;
    private FSItemVM mSelectedItem = null;

    private RelayCommand<object> mSelectionChanged = null;
    private string mSelectedRecentLocation = string.Empty;
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
    public ObservableCollection<string> RecentLocations { get; set; }

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
        }
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
    public void PopulateView()
    {
      ////CurrentItems.Clear();
      string bak = this.CurrentFolder;
      var currentItems = this.CurrentItems;

      this.CurrentItems.Clear();
      this.CurrentFolder = bak;

      // add special folders
      currentItems.Add(
      new FSItemVM()
      {
        FullPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),
        DisplayName = "Desktop",  ////System.IO.Path.GetFileName(s),
        Type = FSItemType.Folder,
        ////DisplayIcon = 
        ////IconExtractor.GetFolderIcon(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory)).ToImageSource()
        ////new BitmapImage(new Uri(@"/FileListView;component/Images/Desktop.png", UriKind.Relative)) //((ShellObject)o).Thumbnail.SmallBitmapSource//Etier.IconHelper.IconReader.GetFolderIcon(s, Etier.IconHelper.IconReader.IconSize.Small, Etier.IconHelper.IconReader.FolderType.Closed).ToImageSource()
      });

      currentItems.Add(
      new FSItemVM()
      {
        FullPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
        DisplayName = "My Documents", ////System.IO.Path.GetFileName(s),
        Type = FSItemType.Folder,
        ////DisplayIcon = IconExtractor.GetFolderIcon(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), true).ToImageSource()
        ////new BitmapImage(new Uri(@"/FileListView;component/Images/MyDocuments.png", UriKind.Relative)) //((ShellObject)o).Thumbnail.SmallBitmapSource//Etier.IconHelper.IconReader.GetFolderIcon(s, Etier.IconHelper.IconReader.IconSize.Small, Etier.IconHelper.IconReader.FolderType.Closed).ToImageSource()
      });

      // Add a seperator (this are templated via x:Null in ControlTemplate)
      currentItems.Add(null);

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
        FSItemVM info = new FSItemVM()
        {
          FullPath = s,
          DisplayName = s,
          Type = FSItemType.Folder,
          DisplayIcon = IconExtractor.GetFolderIcon(s).ToImageSource()
        };
        currentItems.Add(info);

        // add items under current folder
        if (string.Compare(pathroot, s, true) == 0)
        {
          string[] dirs = this.CurrentFolder.Split(new char[] { System.IO.Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries);
          for (int i = 1; i < dirs.Length; i++)
          {
            string curdir = string.Join(string.Empty + System.IO.Path.DirectorySeparatorChar, dirs, 0, i + 1);
            info = new FSItemVM()
            {
              FullPath = curdir,
              DisplayName = dirs[i],
              Type = FSItemType.Folder,
              DisplayIcon = IconExtractor.GetFolderIcon(curdir, true).ToImageSource(),
              Indentation = i * 10
            };

            this.CurrentItems.Add(info);
          }

          this.SelectedItem = this.CurrentItems[CurrentItems.Count - 1];

          if (this.OnCurrentPathChanged != null)
            this.OnCurrentPathChanged(this, new FolderChangedEventArgs() { FilePath = this.SelectedItem.FullPath });
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
            this.OnCurrentPathChanged(this, new FolderChangedEventArgs() { FilePath = this.SelectedItem.FullPath });
        }
      }

      // Add a seperator (this are templated via x:Null in ControlTemplate)
      currentItems.Add(null);

      // add recent locations
      if (this.RecentLocations != null)
      {
        List<string> fullpaths = new List<string>();    // remember paths to avoid adding duplicates
        foreach (object o in this.RecentLocations)
        {
          string s = o.ToString();
          s = System.IO.Path.GetDirectoryName(s);
          if (!fullpaths.Contains(s))
          {
            fullpaths.Add(s);
            FSItemVM it = new FSItemVM()
            {
              FullPath = s,
              ShowToolTip = true,
              DisplayName = System.IO.Path.GetFileName(s),
              Type = FSItemType.Folder,
              DisplayIcon = IconExtractor.GetFolderIcon(s).ToImageSource()
            };

            if (it.DisplayName.Trim() == string.Empty)
              it.DisplayName = it.FullPath;
            currentItems.Add(it);
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
            if (IsPathDirectory(item.FullPath))
            {
              if (this.OnCurrentPathChanged != null)
              {
                this.OnCurrentPathChanged(this, new FolderChangedEventArgs() { FilePath = item.FullPath });
              }
            }
          }
        }
      }

      // Check if the given parameter is a string and fire a corresponding event if so...
      var paramString = p as string;
      if (paramString != null)
      {
        if (IsPathDirectory(paramString))
        {
          if (this.OnCurrentPathChanged != null)
            this.OnCurrentPathChanged(this, new FolderChangedEventArgs() { FilePath = paramString });
        }
      }
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
    #endregion methods
  }
}
