namespace FileListView.ViewModels
{
  using System;
  using System.Windows.Media;
  using FileListView.Models;

  /// <summary>
  /// The Viewmodel for file system items
  /// </summary>
  public class FSItemVM
  {
    #region fields
    private ImageSource mDisplayIcon = null;
    #endregion fields

    #region properties
    public string DisplayName { get; set; }

    public string FullPath { get; set; }

    public FSItemType Type { get; set; }

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

      set
      {
        this.mDisplayIcon = value;
      }
    }

    public bool ShowToolTip { get; set; }

    public int Indentation { get; set; }
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
    #endregion methods
  }
}
