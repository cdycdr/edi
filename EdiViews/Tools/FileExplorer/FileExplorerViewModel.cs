namespace EdiViews.Tools.FileExplorer
{
  using System;
  using System.IO;
  using System.Windows;
  using System.Windows.Input;
  using Edi.ViewModel.Base;
  using EdiViews.ViewModel.Base;
  using FileListView.Command;
  using FileListView.ViewModels;

  /// <summary>
  /// This class can be used to present file based information, such as,
  /// Size, Path etc to the user.
  /// </summary>
  public class FileExplorerViewModel : EdiViews.ViewModel.Base.ToolViewModel
  {
    #region fields
    public const string ToolContentId = "FileExplorerTool";
    private string mFilePathName = string.Empty;

    private Func<string, bool> mFileOpenMethod = null;
    
    private RelayCommand<object> mSyncPathWithCurrentDocumentCommand = null;   
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public FileExplorerViewModel(Func<string, bool> fileOpenMethod = null)
      : base("File Explorer")
    {
      base.ContentId = ToolContentId;

      this.FolderView = new FolderListViewModel(this.FolderItemsView_OnFileOpen);

      this.FolderView.AddRecentFolder( @"C:\temp\");
      this.FolderView.AddRecentFolder( @"C:\windows\");

      this.FolderView.AddFilter("XML", "*.aml;*.xml;*.xsl;*.xslt;*.xsd;*.config;*.addin;*.wxs;*.wxi;*.wxl;*.build;*.xfrm;*.targets;*.xpt;*.xft;*.map;*.wsdl;*.disco;*.ps1xml;*.nuspec");
      this.FolderView.AddFilter("C#", "*.cs;*.manifest;*.resx;*.xaml");
      this.FolderView.AddFilter("Edi", "*.xshd");
      this.FolderView.AddFilter("JavaScript", "*.js");
      this.FolderView.AddFilter("HTML", "*.htm;*.html");
      this.FolderView.AddFilter("ActionScript3", "*.as");
      this.FolderView.AddFilter("ASP/XHTML", "*.asp;*.aspx;*.asax;*.asmx;*.ascx;*.master");
      this.FolderView.AddFilter("Boo", "*.boo");
      this.FolderView.AddFilter("Coco", "*.atg");
      this.FolderView.AddFilter("C++", "*.c;*.h;*.cc;*.cpp;*.hpp;*.rc");
      this.FolderView.AddFilter("CSS", "*.css");
      this.FolderView.AddFilter("BAT", "*.bat;*.dos");
      this.FolderView.AddFilter("F#", "*.fs");
      this.FolderView.AddFilter("INI", "*.cfg;*.conf;*.ini;*.iss;");
      this.FolderView.AddFilter("Java", "*.java");
      this.FolderView.AddFilter("Scheme", "*.sls;*.sps;*.ss;*.scm");
      this.FolderView.AddFilter("LOG", "*.log");
      this.FolderView.AddFilter("MarkDown", "*.md");
      this.FolderView.AddFilter("Patch", "*.patch;*.diff");
      this.FolderView.AddFilter("PowerShell", "*.ps1;*.psm1;*.psd1");
      this.FolderView.AddFilter("Projects", "*.proj;*.csproj;*.drproj;*.vbproj;*.ilproj;*.booproj");
      this.FolderView.AddFilter("Python", "*.py");
      this.FolderView.AddFilter("Ruby", "*.rb");
      this.FolderView.AddFilter("Scheme", "*.sls;*.sps;*.ss;*.scm");
      this.FolderView.AddFilter("StyleCop", "*.StyleCop");
      this.FolderView.AddFilter("SQL", "*.sql");
      this.FolderView.AddFilter("Squirrel", "*.nut");
      this.FolderView.AddFilter("Tex", "*.tex");
      this.FolderView.AddFilter("TXT", "*.txt");
      this.FolderView.AddFilter("VBNET", "*.vb");
      this.FolderView.AddFilter("VTL", "*.vtl;*.vm");
      this.FolderView.AddFilter("All Files", "*.*", true);

      this.mFileOpenMethod = fileOpenMethod;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Expose a viewmodel that controls the combobox folder drop down
    /// and the fodler/file list view.
    /// </summary>
    public FolderListViewModel FolderView { get; set; }

    #region FileName
    public string FileName
    {
      get
      {
        if (string.IsNullOrEmpty(this.mFilePathName) == true)
          return string.Empty;

        try
        {
          return System.IO.Path.GetFileName(mFilePathName);
        }
        catch (Exception)
        {
          return string.Empty;
        }
      }
    }
    #endregion

    #region FilePath
    public string FilePath
    {
      get
      {
        if (string.IsNullOrEmpty(this.mFilePathName) == true)
          return string.Empty;

        try
        {
          return System.IO.Path.GetDirectoryName(mFilePathName);
        }
        catch (Exception)
        {
          return string.Empty;
        }
      }
    }
    #endregion

    #region ToolWindow Icon
    public override Uri IconSource
    {
      get
      {
        return new Uri("pack://application:,,,/FileListView;component/Images/Generic/Folder/folderopened_yellow_16.png", UriKind.RelativeOrAbsolute);
      }
    }
    #endregion ToolWindow Icon

    #region Commands
    /// <summary>
    /// Can be executed to synchronize the current path with the currently active document.
    /// </summary>
    public ICommand SyncPathWithCurrentDocumentCommand
    {
      get
      {
        if (this.mSyncPathWithCurrentDocumentCommand == null)
          this.mSyncPathWithCurrentDocumentCommand = new RelayCommand<object>(
            (p) => this.SyncPathWithCurrentDocumentCommand_Executed(),
            (p) => string.IsNullOrEmpty(this.mFilePathName) == false);

        return this.mSyncPathWithCurrentDocumentCommand;
      }
    }
    #endregion Commands
    #endregion properties

    #region methods
    public void OnActiveDocumentChanged(object sender, DocumentChangedEventArgs e)
    {
      this.mFilePathName = string.Empty;

      if (e != null)
      {
        if (e.ActiveDocument != null)
        {
          FileBaseViewModel f = e.ActiveDocument as FileBaseViewModel;

          if (f != null)
          {
            if (File.Exists(f.FilePath) == true)
            {
              var fi = new FileInfo(f.FilePath);

              this.mFilePathName = f.FilePath;

              this.RaisePropertyChanged(() => this.FileName);
              this.RaisePropertyChanged(() => this.FilePath);
            }
          }
        }
      }
    }

    /// <summary>
    /// Executes when the file open event is fired and class was constructed with statndard constructor.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void FolderItemsView_OnFileOpen(object sender, FileListView.Events.FileOpenEventArgs e)
    {
      if (this.mFileOpenMethod != null)
        this.mFileOpenMethod(e.FileName);
      else
        MessageBox.Show("File Open (method is to null):" + e.FileName);
    }
    
    private void SyncPathWithCurrentDocumentCommand_Executed()
    {
      if (string.IsNullOrEmpty(this.mFilePathName) == true)
        return;

      string directoryPath = string.Empty;
      try
      {
        if (System.IO.Directory.Exists(this.mFilePathName) == true)
          directoryPath = this.mFilePathName;
        else
          directoryPath = System.IO.Directory.GetParent(this.mFilePathName).FullName;

        if (System.IO.Directory.Exists(directoryPath) == false)
          return;
      }
      catch
      {
      }

      this.FolderView.NavigateToFolder(directoryPath);
    }
    #endregion methods
  }
}
