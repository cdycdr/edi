namespace FileSystemModels.Models
{
  /// <summary>
  /// Determine whether a file system item is a folder or a file.
  /// </summary>
  public enum FSItemType
  {
    /// <summary>
    /// File system item is a folder.
    /// </summary>
    Folder,

    /// <summary>
    /// File system item is a file.
    /// </summary>
    File,

    Unknown
  }
}
