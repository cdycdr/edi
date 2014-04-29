namespace FileListView.Events
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;

  /// <summary>
  /// Class implements ...
  /// </summary>
  public class FolderChangedEventArgs : EventArgs
  {
    /// <summary>
    /// Path of directory...
    /// </summary>
    public string FilePath { get; set; }
  }
}
