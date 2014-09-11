namespace Edi.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Globalization;
  using System.IO;
  using System.Linq;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Threading;
  using Edi.ViewModel.Base;
  using EdiViews;
  using EdiViews.Dialogs.About;
  using EdiViews.Dialogs.Config.ViewModel;
  using EdiViews.Documents.Log4Net;
  using EdiViews.ViewModel.Documents.StartPage;
  using EdiViews.Process;
  using EdiViews.Tools.FileExplorer;
  using EdiViews.Tools.FileStats;
  using EdiViews.Tools.Log4Net;
  using EdiViews.Tools.RecentFiles;
  using EdiViews.ViewModel.Base;
  using EdiViews.ViewModel.Base.Events;
  using EdiViews.ViewModel.Documents;
  using Microsoft.Win32;
  using MiniUML.Model.ViewModels.Document;
  using MsgBox;
  using Settings;
  using SimpleControls.MRU.ViewModel;

  /// <summary>
  /// Determine whether an error on load file operation
  /// is silent (no user notification) or not.
  /// </summary>
  public enum CloseDocOnError
  {
    /// <summary>
    /// Close documents automatically without message (when re-loading on startup)
    /// </summary>
    WithoutUserNotification,

    /// <summary>
    /// Close documents on error with message (when doin' interactive File>Open)
    /// </summary>
    WithUserNotification
  }

  public partial class ApplicationViewModel : Edi.ViewModel.Base.ViewModelBase, IMiniUMLDocument
  {
    #region fields
    public const string Log4netFileExtension = "log4j";
    public static readonly string Log4netFileFilter = Util.Local.Strings.STR_FileType_FileFilter_Log4j;

    public const string MiniUMLFileExtension = "uml";
    public static readonly string UMLFileFilter = Util.Local.Strings.STR_FileType_FileFilter_UML;

    public static readonly string EdiTextEditorFileFilter =
                  Util.Local.Strings.STR_FileType_FileFilter_AllFiles +
                  "|" + Util.Local.Strings.STR_FileType_FileFilter_TextFiles +
                  "|" + Util.Local.Strings.STR_FileType_FileFilter_CSharp +
                  "|" + Util.Local.Strings.STR_FileType_FileFilter_HTML +
                  "|" + Util.Local.Strings.STR_FileType_FileFilter_SQL +
                  "|" + Util.Local.Strings.STR_FileType_FileFilter_Log4NetPlusText;

    public const string LayoutFileName = "Layout.config";

    protected static ApplicationViewModel mThis = new ApplicationViewModel();

    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private bool? mDialogCloseResult = null;
    private bool? mIsNotMaximized = null;

    private bool mShutDownInProgress = false;
    private bool mShutDownInProgress_Cancel = false;

    private ObservableCollection<FileBaseViewModel> mFiles = null;
    private ReadOnlyObservableCollection<FileBaseViewModel> mReadonyFiles = null;
    private ToolViewModel[] mTools = null;

    private FileStatsViewModel mFileStats = null;      // Tool Window Properties
    private RecentFilesViewModel mRecentFiles = null;
    private Log4NetToolViewModel mLog4NetTool = null;
    private Log4NetMessageToolViewModel mLog4NetMessageTool = null;
    private FileExplorerViewModel mFileExplorer = null;

    private AvalonDockLayoutViewModel mAVLayout = null;
    private FileBaseViewModel mActiveDocument = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Constructor
    /// </summary>
    protected ApplicationViewModel()
    {
      this.mFiles = new ObservableCollection<FileBaseViewModel>();
    }
    #endregion constructor

    #region Properties
    #region ActiveDocument
    public FileBaseViewModel ActiveDocument
    {
      get
      {
        return this.mActiveDocument;
      }

      set
      {
        if (this.mActiveDocument != value)
        {
          this.mActiveDocument = value;

          this.NotifyPropertyChanged(() => this.ActiveDocument);
          this.NotifyPropertyChanged(() => this.ActiveEdiDocument);
          this.NotifyPropertyChanged(() => this.vm_DocumentViewModel);

          // Ensure that no pending calls are in the dispatcher queue
          // This makes sure that we are blocked until bindings are re-established
          // (Bindings are, for example, required to scroll a selection into view for search/replace)
          Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate
          {
            if (ActiveDocumentChanged != null)
            {
              ActiveDocumentChanged(this, new DocumentChangedEventArgs(this.mActiveDocument)); //this.ActiveDocument

              if (value != null && this.mShutDownInProgress == false)
              {
                if (value.IsFilePathReal == true)
                  SettingsManager.Instance.SessionData.LastActiveFile = value.FilePath;
              }
            }
          });
        }
      }
    }

    public event DocumentChangedEventHandler ActiveDocumentChanged;

    /// <summary>
    /// This is a type safe ActiveDocument property that is used to bind
    /// to an ActiveDocument of type <seealso cref="EdiViewModel"/>.
    /// This property returns null (thus avoiding binding errors) if the
    /// ActiveDocument is not of <seealso cref="EdiViewModel"/> type.
    /// </summary>
    public EdiViewModel ActiveEdiDocument
    {
      get
      {
        return this.mActiveDocument as EdiViewModel;
      }
    }

    /// <summary>
    /// This is a type safe ActiveDocument property that is used to bind
    /// to an ActiveDocument of type <seealso cref="MiniUML.Model.ViewModels.DocumentViewModel"/>.
    /// This property returns null (thus avoiding binding errors) if the
    /// ActiveDocument is not of <seealso cref="MiniUML.Model.ViewModels.DocumentViewModel"/> type.
    /// 
    /// This particular property is also required to load MiniUML Plugins.
    /// </summary>
    public MiniUML.Model.ViewModels.Document.AbstractDocumentViewModel vm_DocumentViewModel
    {
      get
      {
        MiniUmlViewModel vm = this.mActiveDocument as MiniUmlViewModel;

        if (vm != null)
        {
          return vm.DocumentMiniUML as MiniUML.Model.ViewModels.Document.AbstractDocumentViewModel;
        }

        return null;
      }
    }
    #endregion

    /// <summary>
    /// Global static <seealso cref="Workspace" property to make this app root global accessible/>
    /// </summary>
    public static ApplicationViewModel This
    {
      get { return ApplicationViewModel.mThis; }
    }

    /// <summary>
    /// Principable data source for collection of documents managed in the the document manager (of AvalonDock).
    /// </summary>
    public ReadOnlyObservableCollection<FileBaseViewModel> Files
    {
      get
      {
        if (mReadonyFiles == null)
          mReadonyFiles = new ReadOnlyObservableCollection<FileBaseViewModel>(this.mFiles);

        return mReadonyFiles;
      }
    }

    /// <summary>
    /// Convienance property to filter (cast) documents that represent
    /// actual text documents out of the general documents collection.
    /// 
    /// Items such as start page or program settings are not considered
    /// documents in this collection.
    /// </summary>
    private List<EdiViewModel> Documents
    {
      get
      {
        return this.mFiles.OfType<EdiViewModel>().ToList();
      }
    }

    #region Tools
    /// <summary>
    /// Principable data source for collection of tool windows
    /// managed in the the document manager (of AvalonDock).
    /// </summary>
    public IEnumerable<ToolViewModel> Tools
    {
      get
      {
        if (mTools == null)
          mTools = new ToolViewModel[] { this.RecentFiles,
                                         this.FileStats,
                                         this.Log4NetTool, Log4NetMessageTool,
                                         this.FileExplorer
                                        };

        return mTools;
      }
    }

    /// <summary>
    /// Gets the File States ToolWindow ViewModel.
    /// based on the <seealso cref="FileStatsViewModel"/>.
    /// </summary>
    public FileStatsViewModel FileStats
    {
      get
      {
        if (this.mFileStats == null)
        {
          this.mFileStats = new FileStatsViewModel();
          ApplicationViewModel.This.ActiveDocumentChanged += new DocumentChangedEventHandler(this.mFileStats.OnActiveDocumentChanged);
        }

        return this.mFileStats;
      }
    }

    /// <summary>
    /// Gets the data visible in the Recent Files ToolWindow ViewModel.
    /// based on the <seealso cref="RecentFilesViewModel"/>.
    /// </summary>
    public RecentFilesViewModel RecentFiles
    {
      get
      {
        if (this.mRecentFiles == null)
          this.mRecentFiles = new RecentFilesViewModel(SettingsManager.Instance.SessionData.MruList);

        return this.mRecentFiles;
      }
    }

    /// <summary>
    /// Gets the Log4Net Tool Window ViewModel.
    /// based on the <seealso cref="Log4NetToolViewModel"/>.
    /// </summary>
    public Log4NetToolViewModel Log4NetTool
    {
      get
      {
        if (this.mLog4NetTool == null)
        {
          this.mLog4NetTool = new Log4NetToolViewModel();
          ApplicationViewModel.This.ActiveDocumentChanged += new DocumentChangedEventHandler(this.mLog4NetTool.OnActiveDocumentChanged);
        }

        return mLog4NetTool;
      }
    }

    /// <summary>
    /// Gets the Log4NetMessageTool (toolwindow) viewmodel.
    /// </summary>
    public Log4NetMessageToolViewModel Log4NetMessageTool
    {
      get
      {
        if (this.mLog4NetMessageTool == null)
        {
          this.mLog4NetMessageTool = new Log4NetMessageToolViewModel();
          ApplicationViewModel.This.ActiveDocumentChanged += new DocumentChangedEventHandler(this.mLog4NetMessageTool.OnActiveDocumentChanged);
        }

        return this.mLog4NetMessageTool;
      }
    }

    /// <summary>
    /// Gets the Explorer ToolWindow ViewModel.
    /// based on the <seealso cref="FileExplorerViewModel"/>.
    /// </summary>
    public FileExplorerViewModel FileExplorer
    {
      get
      {
        if (this.mFileExplorer == null)
        {
          this.mFileExplorer = new FileExplorerViewModel(this.FileOpen);

          ApplicationViewModel.This.ActiveDocumentChanged += new DocumentChangedEventHandler(this.mFileExplorer.OnActiveDocumentChanged);
        }

        return this.mFileExplorer;
      }
    }
    #endregion Tools

    /// <summary>
    /// Expose command to load/save AvalonDock layout on application startup and shut-down.
    /// </summary>
    public AvalonDockLayoutViewModel ADLayout
    {
      get
      {
        if (this.mAVLayout == null)
          this.mAVLayout = new AvalonDockLayoutViewModel();

        return this.mAVLayout;
      }
    }
    
    internal bool ShutDownInProgress_Cancel
    {
      get
      {
        return this.mShutDownInProgress_Cancel;
      }

      set
      {
        if (this.mShutDownInProgress_Cancel != value)
          this.mShutDownInProgress_Cancel = value;
      }
    }

    #region ApplicationName
    /// <summary>
    /// Get the name of this application in a human read-able fashion
    /// </summary>
    public string ApplicationTitle
    {
      get
      {
        return App.AssemblyTitle;
      }
    }
    #endregion ApplicationName
    #endregion Properties

    #region methods
    /// <summary>
    /// Wrapper method for file open
    /// - is executed when a file open is requested from external party such as tool window.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    private bool FileOpen(string file)
    {
      this.Open(file);
      return true;
    }

    /// <summary>
    /// Open a file supplied in <paramref name="filePath"/> (without displaying a file open dialog).
    /// </summary>
    /// <param name="filePath">file to open</param>
    /// <param name="AddIntoMRU">indicate whether file is to be added into MRU or not</param>
    /// <returns></returns>
    public FileBaseViewModel Open(string filePath,
                                  CloseDocOnError closeDocumentWithoutMessageOnError = CloseDocOnError.WithUserNotification,
                                  bool AddIntoMRU = true,
                                  TypeOfDocument t = TypeOfDocument.EdiTextEditor)
    {
      logger.InfoFormat("TRACE EdiViewModel.Open param: '{0}', AddIntoMRU {1}", filePath, AddIntoMRU);

      // Verify whether file is already open in editor, and if so, show it
      FileBaseViewModel fileViewModel = this.Documents.FirstOrDefault(fm => fm.FilePath == filePath);

      if (fileViewModel != null)
      {
        this.ActiveDocument = fileViewModel; // File is already open so show it to the user

        return fileViewModel;
      }

      string fileExtension = System.IO.Path.GetExtension(filePath);

      if ((fileExtension == string.Format(".{0}", ApplicationViewModel.Log4netFileExtension) && t == TypeOfDocument.EdiTextEditor) ||  t == TypeOfDocument.Log4NetView)
      {
        // try to load a standard log4net XML file from the file system
        fileViewModel = Log4NetViewModel.LoadFile(filePath);
      }
      else
      {
        if ((fileExtension == string.Format(".{0}", ApplicationViewModel.MiniUMLFileExtension) && t == TypeOfDocument.EdiTextEditor) || t == TypeOfDocument.UMLEditor)
	      {
          fileViewModel = MiniUmlViewModel.LoadFile(filePath);
	      }
        else
        {
          bool closeOnErrorWithoutMessage = false;

          if (closeDocumentWithoutMessageOnError == CloseDocOnError.WithoutUserNotification)
            closeOnErrorWithoutMessage = true;

          // try to load a standard text file from the file system
          fileViewModel = EdiViewModel.LoadFile(filePath, closeOnErrorWithoutMessage);
        }
      }

      if (fileViewModel == null)
      {

        if (SettingsManager.Instance.SessionData.MruList.FindMRUEntry(filePath) != null)
        {
          if (MsgBox.Msg.Show(string.Format(Util.Local.Strings.STR_ERROR_LOADING_FILE_MSG, filePath),
                                            Util.Local.Strings.STR_ERROR_LOADING_FILE_CAPTION, MsgBoxButtons.YesNo) == MsgBoxResult.Yes)
          {
            SettingsManager.Instance.SessionData.MruList.RemoveEntry(filePath);
          }
        }

        return null;
      }

      fileViewModel.DocumentEvent += this.ProcessDocumentEvent;
      this.mFiles.Add(fileViewModel);

      // reset viewmodel options in accordance to current program settings
      var ediVM = fileViewModel as EdiViewModel;

      if (ediVM != null)
      {
        ediVM.ProcessingResultEvent += this.vm_ProcessingResultEvent;
        this.SetActiveDocumentOnNewFileOrOpenFile(ediVM);
      }
      else
      {
        this.SetActiveFileBaseDocument(fileViewModel);
      }

      if (AddIntoMRU == true)
        this.RecentFiles.AddNewEntryIntoMRU(filePath);

      return fileViewModel;
    }

    #region NewCommand
    private void OnNew(TypeOfDocument t = TypeOfDocument.EdiTextEditor)
    {
      try
      {
        switch (t)
        {
          case TypeOfDocument.EdiTextEditor:
          {
            var vm = new EdiViewModel();
            vm.InitInstance(Settings.SettingsManager.Instance.SettingData);

            vm.IncreaseNewCounter();
            vm.DocumentEvent += this.ProcessDocumentEvent;
            vm.ProcessingResultEvent += vm_ProcessingResultEvent;
            vm.CreateNewDocument();

            this.mFiles.Add(vm);
            this.SetActiveDocumentOnNewFileOrOpenFile(vm);
          }
          break;

          case TypeOfDocument.UMLEditor:
          {
            var vm = new MiniUmlViewModel();

            vm.DocumentEvent += this.ProcessDocumentEvent;
            this.mFiles.Add(vm);
            this.SetActiveFileBaseDocument(vm);
          }
          break;

          case TypeOfDocument.Log4NetView:
          default:
            throw new NotImplementedException(t.ToString());
        }
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }
    }
    #endregion NewCommand

    #region OpenCommand
    private void OnOpen(TypeOfDocument t = TypeOfDocument.EdiTextEditor)
    {
      try
      {
        var dlg = new OpenFileDialog();

        switch (t)
        {
          case TypeOfDocument.EdiTextEditor:
            dlg.Filter = ApplicationViewModel.EdiTextEditorFileFilter;
            break;

          case TypeOfDocument.Log4NetView:
            dlg.Filter = ApplicationViewModel.Log4netFileFilter;
            break;

          case TypeOfDocument.UMLEditor:
            dlg.Filter = ApplicationViewModel.UMLFileFilter;
            break;

          default:
            throw new NotImplementedException(t.ToString());
        }

        dlg.Multiselect = true;
        dlg.InitialDirectory = this.GetDefaultPath();

        if (dlg.ShowDialog().GetValueOrDefault())
        {
          foreach(string fileName in dlg.FileNames)
          {
            this.Open(fileName, CloseDocOnError.WithUserNotification, true, t);
          }
        }
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }
    }
    #endregion OnOpen

    #region Application_Exit_Command
    private void AppExit_CommandExecuted()
    {
      try
      {
        if (this.Closing_CanExecute() == true)
        {
          this.mShutDownInProgress_Cancel = false;
          this.OnRequestClose();
        }
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }
    }
    #endregion Application_Exit_Command

    private void AppProgramSettings_CommandExecuted()
    {
      try
      {
        // Initialize view model for editing settings
        ConfigViewModel dlgVM = new ConfigViewModel();
        dlgVM.LoadOptionsFromModel(SettingsManager.Instance.SettingData);

        // Create dialog and attach viewmodel to view datacontext
        Window dlg = ViewSelector.GetDialogView(dlgVM, Application.Current.MainWindow);

        dlg.ShowDialog();

        if (dlgVM.WindowCloseResult == true)
        {
          dlgVM.SaveOptionsToModel(SettingsManager.Instance.SettingData);

          if (SettingsManager.Instance.SettingData.IsDirty == true)
            SettingsManager.Instance.SaveOptions(App.DirFileAppSettingsData, SettingsManager.Instance.SettingData);
        }
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }
    }

    #region Application_About_Command
    private void AppAbout_CommandExecuted()
    {
      try
      {
        var vm = new AboutViewModel();
        Window dlg = ViewSelector.GetDialogView(vm, Application.Current.MainWindow);

        dlg.ShowDialog();
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }
    }
    #endregion Application_About_Command

    #region Recent File List Pin Unpin Commands
    private void PinCommand_Executed(object o, ExecutedRoutedEventArgs e)
    {
      try
      {
        var cmdParam = o as MRUEntryVM;

        if (cmdParam == null)
          return;

        if (e != null)
          e.Handled = true;

        this.RecentFiles.MruList.PinUnpinEntry(!cmdParam.IsPinned, cmdParam);
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }
    }

    private void AddMRUEntry_Executed(object o, ExecutedRoutedEventArgs e)
    {
      try
      {
        var cmdParam = o as MRUEntryVM;

        if (cmdParam == null)
          return;

        if (e != null)
          e.Handled = true;

        this.RecentFiles.MruList.AddMRUEntry(cmdParam);
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }
    }

    private void RemoveMRUEntry_Executed(object o, ExecutedRoutedEventArgs e)
    {
      try
      {
        var cmdParam = o as MRUEntryVM;

        if (cmdParam == null)
          return;

        if (e != null)
          e.Handled = true;

        this.RecentFiles.MruList.RemovePinEntry(cmdParam);
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }
    }
    #endregion Recent File List Pin Unpin Commands

    #region RequestClose [event]

    /// <summary>
    /// Raised when this workspace should be removed from the UI.
    /// </summary>
    public event EventHandler RequestClose;

    /// <summary>
    /// Method to be executed when user (or program) tries to close the application
    /// </summary>
    public void OnRequestClose()
    {
      try
      {
        if (this.mShutDownInProgress == false)
        {
          if (this.DialogCloseResult == null)
            this.DialogCloseResult = true;      // Execute Close event via attached property

          if (this.mShutDownInProgress_Cancel == true)
          {
            this.mShutDownInProgress = false;
            this.mShutDownInProgress_Cancel = false;
            this.DialogCloseResult = null;
          }
          else
          {
            this.mShutDownInProgress = true;

            CommandManager.InvalidateRequerySuggested();

            EventHandler handler = this.RequestClose;

            if (handler != null)
              handler(this, EventArgs.Empty);
          }
        }
      }
      catch (Exception exp)
      {
        this.mShutDownInProgress = false;

        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }
    }
    #endregion // RequestClose [event]

    private void SetActiveFileBaseDocument(FileBaseViewModel vm)
    {
      try
      {
        this.ActiveDocument = vm;
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }
    }

    /// <summary>
    /// Reset file view options in accordance with current program settings
    /// whenever a new file is internally created (on File Open or New File)
    /// </summary>
    /// <param name="vm"></param>
    private void SetActiveDocumentOnNewFileOrOpenFile(EdiViewModel vm)
    {
      try
      {
        // Set scale factor in default size of text font
        vm.InitScaleView(SettingsManager.Instance.SettingData.DocumentZoomUnit,
                         SettingsManager.Instance.SettingData.DocumentZoomView);

        this.ActiveDocument = vm;
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }
    }

    /// <summary>
    /// Implement part of requirement § 3.1.0 
    /// 
    /// The Open/SaveAs file dialog opens in the location of the currently active document (if any).
    /// 
    /// Otherwise, if there is no active document or the active document has never been saved before,
    /// the location of the last file open or file save/save as (which ever was last)
    /// is displayed in the Open/SaveAs File dialog.
    /// 
    /// The Open/SaveAs file dialog opens in the MyDocuments Windows user folder
    /// if none of the above conditions are true. (eg.: Open file for the very first
    /// time or last location does not exist).
    /// 
    /// The Open/Save/SaveAs file dialog opens in "C:\" if none of the above requirements
    /// can be implemented (eg.: MyDocuments folder does not exist or user has no access).
    /// 
    /// The last Open/Save/SaveAs file location used is stored and recovered between user sessions.
    /// </summary>
    /// <returns></returns>
    private string GetDefaultPath()
    {
      string sPath = string.Empty;

      try
      {
        // Generate a default path from cuurently or last active document
        if (this.ActiveEdiDocument != null)
          sPath = this.ActiveEdiDocument.GetFilePath();

        if (sPath == string.Empty)
          sPath = SettingsManager.Instance.SessionData.GetLastActivePath();

        if (sPath == string.Empty)
          sPath = App.MyDocumentsUserDir;
        else
        {
          try
          {
            if (System.IO.Directory.Exists(sPath) == false)
              sPath = App.MyDocumentsUserDir;
          }
          catch
          {
            sPath = App.MyDocumentsUserDir;
          }
        }
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }

      return sPath;
    }

    /// <summary>
    /// Attempt to save data in file when
    /// File>Save As... or File>Save command
    /// is executed.
    /// </summary>
    /// <param name="doc"></param>
    /// <param name="saveAsFlag"></param>
    /// <returns></returns>
    internal bool OnSave(FileBaseViewModel doc, bool saveAsFlag = false)
    {
      if (doc == null)
        return false;

      if (doc.CanSaveData == true)
        return this.OnSaveDocumentFile(doc, saveAsFlag, ApplicationViewModel.GetDefaultFileFilter(doc));

      throw new NotSupportedException((doc != null ? doc.ToString() : Util.Local.Strings.STR_MSG_UnknownDocumentType));
    }

    /// <summary>
    /// Returns the default file extension filter strings
    /// that can be used for each corresponding document
    /// type (viewmodel), or an empty string if no document
    /// type (viewmodel) was matched.
    /// </summary>
    /// <param name="f"></param>
    /// <returns></returns>
    internal static string GetDefaultFileFilter(FileBaseViewModel f)
    {
      if (f == null)
        return string.Empty;

      if (f is EdiViewModel)
        return ApplicationViewModel.EdiTextEditorFileFilter;

      if (f is MiniUmlViewModel)
        return ApplicationViewModel.UMLFileFilter;

      if (f is Log4NetViewModel)
        return ApplicationViewModel.Log4netFileExtension;

      return string.Empty;
    }

    internal bool OnSaveDocumentFile(FileBaseViewModel fileToSave,
                                     bool saveAsFlag = false,
                                     string FileExtensionFilter = "")
    {
      string filePath = (fileToSave == null ? string.Empty : fileToSave.FilePath);

      // Offer SaveAs file dialog if file has never been saved before (was created with new command)
      if (fileToSave != null)
        saveAsFlag = saveAsFlag | !fileToSave.IsFilePathReal;

      try
      {
        if (filePath == string.Empty || saveAsFlag == true)   // Execute SaveAs function
        {
          var dlg = new SaveFileDialog();

          try
          {
            dlg.FileName = System.IO.Path.GetFileName(filePath);
          }
          catch
          {
          }

          dlg.InitialDirectory = this.GetDefaultPath();

          if (string.IsNullOrEmpty(FileExtensionFilter) == false)
            dlg.Filter = FileExtensionFilter;

          if (dlg.ShowDialog().GetValueOrDefault() == true)     // SaveAs file if user OK'ed it so
          {
            filePath = dlg.FileName;

            fileToSave.SaveFile(filePath);
          }
          else
            return false;
        }
        else                                                  // Execute Save function
          fileToSave.SaveFile(filePath);

        this.RecentFiles.AddNewEntryIntoMRU(filePath);

        return true;
      }
      catch (Exception Exp)
      {
        string sMsg = Util.Local.Strings.STR_MSG_ErrorSavingFile;

        if (filePath.Length > 0)
          sMsg = string.Format(CultureInfo.CurrentCulture, Util.Local.Strings.STR_MSG_ErrorWhileSavingFileX, Exp.Message, filePath);
        else
          sMsg = string.Format(CultureInfo.CurrentCulture, Util.Local.Strings.STR_MSG_ErrorWhileSavingAFile, Exp.Message);

        MsgBox.Msg.Show(sMsg, Util.Local.Strings.STR_MSG_ErrorSavingFile, MsgBoxButtons.OK);
      }

      return false;
    }

    internal bool OnCloseSaveDirtyFile(FileBaseViewModel fileToClose)
    {
      if (fileToClose.IsDirty == true &&
          fileToClose.CanSaveData == true)
      {
        var res = MsgBox.Msg.Show(string.Format(CultureInfo.CurrentCulture, Util.Local.Strings.STR_MSG_SaveChangesForFile, fileToClose.FileName),
                                  this.ApplicationTitle,
                                  MsgBoxButtons.YesNoCancel, MsgBoxImage.Question,
                                  MsgBoxResult.Yes, false, 
                                  MsgBoxResult.Yes);

        if (res == MsgBoxResult.Cancel)
          return false;

        if (res == MsgBoxResult.Yes)
        {
          return OnSave(fileToClose);
        }
      }

      return true;
    }

    /// <summary>
    /// Close the currently active file and set the file with the lowest index as active document.
    /// 
    /// TODO: The last active document that was active before the document being closed should be activated next.
    /// </summary>
    /// <param name="fileToClose"></param>
    /// <returns></returns>
    internal bool Close(FileBaseViewModel doc)
    {
      try
      {
        {
          if (this.OnCloseSaveDirtyFile(doc) == false)
            return false;

          doc.DocumentEvent -= this.ProcessDocumentEvent;

          int idx = this.mFiles.IndexOf(doc);

          this.mFiles.Remove(doc);

          if (this.Documents.Count > idx)
            this.ActiveDocument = this.mFiles[idx];
          else
            if (this.Documents.Count > 1 && this.Documents.Count == idx)
              this.ActiveDocument = this.mFiles[idx-1];
          else
          if (this.Documents.Count == 0)
            this.ActiveDocument = null;
          else
            this.ActiveDocument = this.mFiles[0];

          return true;
        }

        /*
          // This could be a StartPage, Log4Net, or UML file or any other (read-only) document type
          if (doc != null)
          {
            if (doc.IsDirty == true)
            {
              if (this.OnCloseSaveDirtyFile(doc) == false)
                return false;
            }

            mFiles.Remove(doc);

            if (this.Documents.Count == 0)
              this.ActiveDocument = null;
            else
              this.ActiveDocument = this.mFiles[0];

            return true;
          }
        */
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }

      // Throw an exception if this method does not know how the input document type is to be closed
      throw new NotSupportedException(doc.ToString());
    }

    /// <summary>
    /// This can be used to close the attached view via ViewModel
    /// 
    /// Source: http://stackoverflow.com/questions/501886/wpf-mvvm-newbie-how-should-the-viewmodel-close-the-form
    /// </summary>
    public bool? DialogCloseResult
    {
      get
      {
        return this.mDialogCloseResult;
      }

      private set
      {
        if (this.mDialogCloseResult != value)
        {
          this.mDialogCloseResult = value;
          this.NotifyPropertyChanged(() => this.DialogCloseResult);
        }
      }
    }

    /// <summary>
    /// Get/set property to determine whether window is in maximized state or not.
    /// (this can be handy to determine when a resize grip should be shown or not)
    /// </summary>
    public bool? IsNotMaximized
    {
      get
      {
        return this.mIsNotMaximized;
      }

      set
      {
        if (this.mIsNotMaximized != value)
        {
          this.mIsNotMaximized = value;
          this.NotifyPropertyChanged(() => this.IsNotMaximized);
        }
      }
    }

    /// <summary>
    /// Check if pre-requisites for closing application are available.
    /// Save session data on closing and cancel closing process if necessary.
    /// </summary>
    /// <returns>true if application is OK to proceed closing with closed, otherwise false.</returns>
    internal bool Exit_CheckConditions(object sender)
    {
      if (this.mShutDownInProgress == true)
        return true;

      try
      {
        if (this.mFiles != null)               // Close all open files and make sure there are no unsaved edits
        {                                     // If there are any: Ask user if edits should be saved
          for (int i = 0; i < this.Files.Count; i++)
          {
            FileBaseViewModel f = this.Files[i];

            if (this.OnCloseSaveDirtyFile(f) == false)
            {
              this.mShutDownInProgress = false;
              return false;               // Cancel shutdown process (return false) if user cancels saving edits
            }
          }
        }

        // Do layout serialization after saving/closing files
        // since changes implemented by shut-down process are otherwise lost
        try
        {
          App.CreateAppDataFolder();
          ////this.SerializeLayout(sender);            // Store the current layout for later retrieval
        }
        catch
        {
        }
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }

      return true;
    }

    /// <summary>
    /// Set the active document to the file in <seealso cref="fileNamePath"/>
    /// if this is currently open.
    /// </summary>
    /// <param name="fileNamePath"></param>
    internal bool SetActiveDocument(string fileNamePath)
    {
      try
      {
        if (this.Files.Count >= 0)
        {
          EdiViewModel fi = this.Documents.SingleOrDefault(f => f.FilePath == fileNamePath);

          if (fi != null)
          {
            this.ActiveDocument = fi;
            return true;
          }
        }
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Show(exp, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                        MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                        App.IssueTrackerLink, App.IssueTrackerLink, Util.Local.Strings.STR_MSG_IssueTrackerText, null, true);
      }

      return false;
    }

    /// <summary>
    /// Construct and add a new <seealso cref="StartPageViewModel"/> to intenral
    /// list of documents, if none is already present, otherwise return already
    /// present <seealso cref="StartPageViewModel"/> from internal document collection.
    /// </summary>
    /// <param name="CreateNewViewModelIfNecessary"></param>
    /// <returns></returns>
    internal StartPageViewModel GetStartPage(bool CreateNewViewModelIfNecessary)
    {
      List <StartPageViewModel> l = this.mFiles.OfType<StartPageViewModel>().ToList();

      if (l.Count == 0)
      {
        if (CreateNewViewModelIfNecessary == false)
          return null;
        else
        {
          var s = new StartPageViewModel(SettingsManager.Instance.SessionData.MruList);

          s.DocumentEvent += ProcessDocumentEvent;

          this.mFiles.Add(s);

          return s;
        }
      }

      return l[0];
    }

    /// <summary>
    /// Close document via dedicated event handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void ProcessDocumentEvent(object sender, FileBaseEvent e)
    {
      var f = sender as FileBaseViewModel;

      switch (e.TypeOfEvent)
      {
        case FileEventType.Unknown:
          break;

        case FileEventType.CloseDocument:
          if (f != null)
            this.CloseDocument(f);
          break;

        case FileEventType.AdjustCurrentPath:
          if (f != null)
            this.FileExplorer.NavigateToFolder(f.GetAlternativePath());
          break;

        default:
          break;
      }
    }

    private void CloseDocument(FileBaseViewModel f)
    {
      if (f != null)
      {
        // Detach EdiViewModel specific events
        var eVM = f as EdiViewModel;
        if (eVM != null)
          eVM.ProcessingResultEvent -= vm_ProcessingResultEvent;

        this.Close(f);
      }
    }

    /// <summary>
    /// Handle Processing results from asynchronous tasks that are
    /// executed in a viewmodel and return later with a mResult.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void vm_ProcessingResultEvent(object sender, ProcessResultEvent e)
    {
      var vm = sender as EdiViewModel;

      if (vm != null)
      {
        try
        {
          switch (e.TypeOfResult)
          {
            case TypeOfResult.FileLoad:      // Process an EdiViewModel file load event mResult
              if (e.InnerException != null)
              {
                Exception error = vm.GetInnerMostException(e.InnerException);

                string filePath = vm.FilePath;
                this.CloseDocument(vm);
                vm = null;

                if (error != null && filePath != null)
                {
                  if (error is FileNotFoundException)
                  {
                    if (SettingsManager.Instance.SessionData.MruList.FindMRUEntry(filePath) != null)
                    {
                      if (MsgBox.Msg.Show(string.Format(Util.Local.Strings.STR_ERROR_LOADING_FILE_MSG, filePath),
                                                        Util.Local.Strings.STR_ERROR_LOADING_FILE_CAPTION, MsgBoxButtons.YesNo) == MsgBoxResult.Yes)
                      {
                        SettingsManager.Instance.SessionData.MruList.RemoveEntry(filePath);
                      }
                    }

                    return;
                  }
                }

                MsgBox.Msg.Show(e.InnerException, "An unexpected error occured",
                                 MsgBoxButtons.OK, MsgBoxImage.Alert);
              }
              break;

            default:
              throw new NotImplementedException(e.TypeOfResult.ToString());
          }
        }
        catch (Exception)
        {

          throw;
        }
        finally
        {
        
        }
      }
    }
    #endregion methods
  }
}
