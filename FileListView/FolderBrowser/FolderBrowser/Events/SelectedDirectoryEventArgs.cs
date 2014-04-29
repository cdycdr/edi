namespace FolderBrowser.Events
{
  using System;

  /// <summary>
  /// This event tells the receiver that the user wants to browse to this path.
  /// </summary>
  public class SelectedDirectoryEventArgs : EventArgs
  {
    /// <summary>
    /// Path an file name of file to open.
    /// </summary>
    public string DirectoryPath { get; set; }
  }
}
