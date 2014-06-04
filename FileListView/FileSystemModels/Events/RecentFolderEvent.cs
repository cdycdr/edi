namespace FileSystemModels.Events
{
  using System;
  using FileSystemModels.Models;

  public class RecentFolderEvent : EventArgs
  {
    public enum RecentFolderAction
    {
      Remove = 0,
      Add = 1
    }

    /// <summary>
    /// Event type class constructor from parameter
    /// </summary>
    public RecentFolderEvent(PathModel path,
                             RecentFolderAction action =  RecentFolderAction.Add)
    : this()
    {
      this.Folder = path;
      this.Action = action;
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    public RecentFolderEvent()
    : base()
    {
      this.Folder = null;
      this.Action = RecentFolderAction.Add;
    }

    /// <summary>
    /// Path to this directory...
    /// </summary>
    public PathModel Folder { get; set; }

    public RecentFolderAction Action { get; set; }
  }
}
