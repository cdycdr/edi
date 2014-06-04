namespace FileListView.ViewModels
{
  using System.Windows.Media;
  using FileListView.Utils;
  using FileSystemModels.Models;
  using FileSystemModels.Utils;

  /// <summary>
  /// The Viewmodel for file system items
  /// </summary>
  public class FSItemVM : Base.ViewModelBase
  {
    #region fields
    private ImageSource mDisplayIcon = null;
    private PathModel mPathObject = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// class constructor
    /// </summary>
    /// <param name="curdir"></param>
    /// <param name="displayName"></param>
    /// <param name="itemType"></param>
    /// <param name="showIcon"></param>
    /// <param name="indentation"></param>
    public FSItemVM(string curdir,
                    FSItemType itemType,
                    string displayName,
                    bool showIcon,
                    int indentation = 0)
      : this(curdir, itemType, displayName, indentation)
    {
      this.ShowToolTip = showIcon;
    }

    /// <summary>
    /// class constructor
    /// </summary>
    /// <param name="curdir"></param>
    /// <param name="displayName"></param>
    /// <param name="itemType"></param>
    /// <param name="indentation"></param>
    public FSItemVM(string curdir,
                    FSItemType itemType,
                    string displayName,
                    int indentation = 0)
    {
      this.mPathObject = new PathModel(curdir, itemType);

      this.DisplayName = displayName;
      this.Indentation = indentation;
    }

    /// <summary>
    /// Hidden standard class constructor
    /// </summary>
    protected FSItemVM()
    {
      this.mPathObject = null;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Gets a name that can be used for display
    /// (is not necessarily the same as path)
    /// </summary>
    public string DisplayName { get; private set; }

    /// <summary>
    /// Gets the path to this item
    /// </summary>
    public string FullPath
    {
      get
      {
        return (this.mPathObject != null ? this.mPathObject.Path : null);
      }
    }

    /// <summary>
    /// Gets the type (folder, file) of this item
    /// </summary>
    public FSItemType Type
    {
      get
      {
        return (this.mPathObject != null ? this.mPathObject.PathType : FSItemType.Unknown);
      }
    }

    /// <summary>
    /// Gets a copy of the internal <seealso cref="PathModel"/> object.
    /// </summary>
    public PathModel GetModel
    {
      get
      {
        return new PathModel(this.mPathObject);
      }
    }

    /// <summary>
    /// Gets an icon to display for this item.
    /// </summary>
    public ImageSource DisplayIcon
    {
      get
      {
        if (this.mDisplayIcon == null)
        {
          try
          {
            if (this.Type == FSItemType.Folder)
              this.mDisplayIcon = IconExtractor.GetFolderIcon(this.FullPath).ToImageSource();
            else
              this.mDisplayIcon = IconExtractor.GetFileIcon(this.FullPath).ToImageSource();
          }
          catch
          {

          }
        }

        return this.mDisplayIcon;
      }

      private set
      {
        if (this.mDisplayIcon != value)
        {
          this.mDisplayIcon = value;
        }
      }
    }

    /// <summary>
    /// Gets whether or not to show a tooltip for this item.
    /// </summary>
    public bool ShowToolTip { get; private set; }

    /// <summary>
    /// Gets an indendation (if any) for this item.
    /// An indendation allows the display of path
    /// items
    ///      in
    ///        stair
    ///             like
    ///                 display
    ///                        fashion.
    /// </summary>
    public int Indentation { get; private set; }
    #endregion properties

    #region methods
    /// <summary>
    /// Standard method to display contents of this class.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return this.FullPath;
    }

    /// <summary>
    /// Assign a certain icon to this item.
    /// </summary>
    /// <param name="src"></param>
    public void SetDisplayIcon(ImageSource src = null)
    {
      if (src == null)
        this.DisplayIcon = IconExtractor.GetFolderIcon(this.FullPath, true).ToImageSource();
      else
        this.DisplayIcon = src;
    }

    /// <summary>
    /// Determine whether a given path is an exeisting directory or not.
    /// </summary>
    /// <returns>true if this directory exists and otherwise false</returns>
    public bool DirectoryPathExists()
    {
      return this.mPathObject.DirectoryPathExists();
    }
    #endregion methods
  }
}
