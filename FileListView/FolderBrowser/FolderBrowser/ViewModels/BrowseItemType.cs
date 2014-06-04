namespace FolderBrowser.ViewModels
{
  /// <summary>
  /// Enumeration implements a type folder item that can be browsed to...
  /// </summary>
  public enum BrowseItemType
  {
    /// <summary>
    /// Unknown type of item to browse over.
    /// </summary>
    Unknown = -1,

    /// <summary>
    /// Represents a drive eg: C:\
    /// </summary>
    HardDisk = 0,

    /// <summary>
    /// Represents a folder eg: C:\Windows
    /// </summary>
    Folder = 1,

    ////Computer = 2,
    ////NetworkShare = 3,
  }
}
