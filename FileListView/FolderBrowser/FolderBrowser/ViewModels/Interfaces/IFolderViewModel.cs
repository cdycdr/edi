namespace FolderBrowser.ViewModels.Interfaces
{
  using System.Collections.ObjectModel;
  using System.Windows.Input;

  /// <summary>
  /// Implment the interface for a viewmodel for one folder entry for a collection of folders.
  /// </summary>
  public interface IFolderViewModel
  {
    #region properties
    string FolderName { get; set; }

    /// <summary>
    /// Get/set file system Path for this folder.
    /// </summary>
    string FolderPath { get; set; }

    /// <summary>
    /// Get/set observable collection of sub-folders of this folder.
    /// </summary>
    ObservableCollection<IFolderViewModel> Folders { get; }

    /// <summary>
    /// Get/set whether this folder is currently selected or not.
    /// </summary>
    bool IsSelected { get; set; }

    /// <summary>
    /// Get/set whether this folder is currently expanded or not.
    /// </summary>
    bool IsExpanded { get; set; }

    /// <summary>
    /// Gets the type of this item (eg: Folder, HardDisk etc...).
    /// </summary>
    BrowseItemType ItemType { get; }

    /// <summary>
    /// Gets a command that will open the selected item with the current default application
    /// in Windows. The selected item (path to a file) is expected as <seealso cref="FSItemVM"/> parameter.
    /// (eg: Item is HTML file -> Open in Windows starts the web browser for viewing the HTML
    /// file if thats the currently associated Windows default application.
    /// </summary>
    ICommand OpenInWindowsCommand { get; }

    /// <summary>
    /// Gets a command that will copy the path of an item into the Windows Clipboard.
    /// The item (path to a file) is expected as <seealso cref="FSItemVM"/> parameter.
    /// </summary>
    ICommand CopyPathCommand { get; }
    #endregion properties
  }
}
