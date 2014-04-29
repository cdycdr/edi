namespace FolderBrowser.ViewModel
{
  using System;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Linq;

  /// <summary>
  /// Implment the viewmodel for one folder entry for a collection of folders.
  /// </summary>
  public class FolderViewModel : Base.ViewModelBase
  {
    #region fields
    private bool mIsSelected;
    private bool mIsExpanded;
    private string mFolderIcon;
    private ObservableCollection<FolderViewModel> mFolders = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard <seealso cref="FolderViewModel"/> constructor
    /// </summary>
    public FolderViewModel()
    {
    }
    #endregion constructor

    #region properties
    public BrowserViewModel Root
    {
      get;
      set;
    }

    public string FolderIcon
    {
      get
      {
        return this.mFolderIcon;
      }
      set
      {
        this.mFolderIcon = value;
        this.RaisePropertyChanged(() => this.FolderIcon);
      }
    }

    public string FolderName
    {
      get;
      set;
    }

    /// <summary>
    /// Get/set file system path for this folder.
    /// </summary>
    public string FolderPath
    {
      get;
      set;
    }

    /// <summary>
    /// Get/set observable collection of sub-folders of this folder.
    /// </summary>
    public ObservableCollection<FolderViewModel> Folders
    {
      get
      {
        if (this.mFolders == null)
          this.mFolders = new ObservableCollection<FolderViewModel>();

        return this.mFolders;
      }
    }

    /// <summary>
    /// Get/set whether this folder is currently selected or not.
    /// </summary>
    public bool IsSelected
    {
      get
      {
        return mIsSelected;
      }

      set
      {
        if (this.mIsSelected != value)
        {
          this.mIsSelected = value;

          this.RaisePropertyChanged(() => this.IsSelected);

          if (value == true)
          {
            this.Root.SelectedFolder = FolderPath;
            this.IsExpanded = true;                 //Default windows behaviour of expanding the selected folder
          }
        }
      }
    }

    /// <summary>
    /// Get/set whether this folder is currently expanded or not.
    /// </summary>
    public bool IsExpanded
    {
      get
      {
        return this.mIsExpanded;
      }

      set
      {
        if (this.mIsExpanded != value)
        {
          this.mIsExpanded = value;

          this.RaisePropertyChanged(() => this.IsExpanded);

          // Folder icon change not applicable for drive(s)
          if (this.FolderName.Contains(':') == false)
          {
            if (this.mIsExpanded)
              this.FolderIcon = "Images\\FolderOpen.png";
            else
              this.FolderIcon = "Images\\FolderClosed.png";
          }

          // Load all sub-folders into the Folders collection.
          this.LoadFolders();
        }
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Load all sub-folders into the Folders collection.
    /// </summary>
    private void LoadFolders()
    {
      try
      {
        if (this.Folders.Count > 0)
          return;

        string[] dirs = null;

        string fullPath = Path.Combine(FolderPath, FolderName);

        if (this.FolderName.Contains(':'))                  //This is a drive
          fullPath = string.Concat(FolderName, "\\");
        else
          fullPath = this.FolderPath;

        try
        {
          dirs = Directory.GetDirectories(fullPath);
        }
        catch (Exception)
        {
        }

        this.Folders.Clear();

        if (dirs != null)
        {
          foreach (string dir in dirs)
          {
            try
            {
              DirectoryInfo di = new DirectoryInfo(dir);
              // create the sub-structure only if this is not a hidden directory
              if ((di.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
              {
                this.Folders.Add(new FolderViewModel { Root = this.Root, FolderName = Path.GetFileName(dir), FolderPath = Path.GetFullPath(dir), FolderIcon = "Images\\FolderClosed.png" });
              }
            }
            catch (UnauthorizedAccessException ae)
            {
              Console.WriteLine(ae.Message);
            }
            catch (Exception e)
            {
              Console.WriteLine(e.Message);
            }
          }
        }

        if (this.FolderName.Contains(":"))
          this.FolderIcon = "Images\\HardDisk.ico";
      }
      catch (UnauthorizedAccessException ae)
      {
        Console.WriteLine(ae.Message);
      }
      catch (IOException ie)
      {
        Console.WriteLine(ie.Message);
      }
    }
    #endregion methods
  }
}
