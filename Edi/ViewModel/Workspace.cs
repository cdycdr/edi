namespace Edi.ViewModel
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.Globalization;
  using System.Linq;
  using System.Windows;
  using System.Windows.Input;
  using System.Windows.Threading;

  using AvalonDock.Layout.Serialization;
  using Edi.View;
  using EdiViews.About;
  using EdiViews.ViewModel.Base;
  using Microsoft.Win32;
  using MsgBox;
  using SimpleControls.MRU.ViewModel;

  partial class Workspace : EdiViews.ViewModel.Base.ViewModelBase
  {
    #region fields
    public const string Log4netFileExtension = "log4net";

    public const string FileFilter =  "All Files (*.*)|*.*" +
                                     "|Structured Query Language (*.sql) |*.sql" +
                                     "|Text Files (*.txt)|*.txt" +
                                     "|log4net XML output (*.log4net)|*.log4net";

    public const string LayoutFileName = "Layout.config";

    protected static Workspace mThis = new Workspace();

    protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    private bool? mDialogCloseResult;
    private bool mShutDownInProgress;
    private bool mShutDownInProgress_Cancel;

    private ObservableCollection<FileBaseViewModel> mFiles = null;
    private ReadOnlyObservableCollection<FileBaseViewModel> mReadonyFiles = null;
    private ToolViewModel[] mTools = null;

    private FileStatsViewModel mFileStats = null;      // Tool Window Properties
    private RecentFilesViewModel mRecentFiles = null;
    private Log4NetToolViewModel mLog4NetTool = null;
    private Log4NetMessageToolViewModel mLog4NetMessageTool = null;

    private FileBaseViewModel mActiveDocument = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Constructor
    /// </summary>
    protected Workspace()
    {
      this.mFiles = new ObservableCollection<FileBaseViewModel>();
      this.mDialogCloseResult = null;
      this.mShutDownInProgress = mShutDownInProgress_Cancel = false;
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

          // Ensure that no pending calls are in the dispatcher queue
          // This makes sure that we are blocked until bindings are re-established
          // (Bindings are, for example, required to scroll a selection into view for search/replace)
          Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate
          {
            if (ActiveDocumentChanged != null)
            {
              ActiveDocumentChanged(this, EventArgs.Empty);

              if (value != null && this.mShutDownInProgress == false)
              {
                if (value.IsFilePathReal == true)
                  this.Config.LastActiveFile = value.FilePath;
              }
            }
          });
        }
      }
    }

    public event EventHandler ActiveDocumentChanged;

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

    #endregion

    /// <summary>
    /// Global static <seealso cref="Workspace" property to make this app root global accessible/>
    /// </summary>
    public static Workspace This
    {
      get { return Workspace.mThis; }
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

    /// <summary>
    /// Principable data source for collection of tool windows managed in the the document manager (of AvalonDock).
    /// </summary>
    public IEnumerable<ToolViewModel> Tools
    {
      get
      {
        if (mTools == null)
          mTools = new ToolViewModel[] { this.RecentFiles,
                                         this.FileStats,
                                         this.Log4NetTool, Log4NetMessageTool
                                        };

        return mTools;
      }
    }

    /// <summary>
    /// This property manages the data visible in the File States View
    /// based on the <seealso cref="FileStatsViewModel"/>.
    /// </summary>
    public FileStatsViewModel FileStats
    {
      get
      {
        if (this.mFileStats == null)
          this.mFileStats = new FileStatsViewModel();

        return this.mFileStats;
      }
    }

    /// <summary>
    /// This property manages the data visible in the Recent Files View
    /// based on the <seealso cref="RecentFilesViewModel"/>.
    /// </summary>
    public RecentFilesViewModel RecentFiles
    {
      get
      {
        if (this.mRecentFiles == null)
          this.mRecentFiles = new RecentFilesViewModel();

        return this.mRecentFiles;
      }
    }

    /// <summary>
    /// This property manages the data visible in the Log4Net Tool Window View
    /// based on the <seealso cref="Log4NetToolViewModel"/>.
    /// </summary>
    public Log4NetToolViewModel Log4NetTool
    {
      get
      {
        if (this.mLog4NetTool == null)
          this.mLog4NetTool = new Log4NetToolViewModel();

        return mLog4NetTool;
      }
    }

    public Log4NetMessageToolViewModel Log4NetMessageTool
    {
      get
      {
        if (this.mLog4NetMessageTool == null)
          this.mLog4NetMessageTool = new Log4NetMessageToolViewModel();

        return this.mLog4NetMessageTool;
      }
    }
    

    public bool ShutDownInProgress_Cancel
    {
      get
      {
        return this.mShutDownInProgress_Cancel;
      }

      set
      {
        if (this.mShutDownInProgress_Cancel != value)
        {
          this.mShutDownInProgress_Cancel = value;
          this.NotifyPropertyChanged(() => this.mShutDownInProgress_Cancel);
        }
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
    /// Open a file supplied in <paramref name="filePath"/> (without displaying a file open dialog).
    /// </summary>
    /// <param name="filePath">file to open</param>
    /// <param name="AddIntoMRU">indicate whether file is to be added into MRU or not</param>
    /// <returns></returns>
    public FileBaseViewModel Open(string filePath, bool AddIntoMRU = true)
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

      if (fileExtension == string.Format(".{0}", Workspace.Log4netFileExtension))
      {
        // try to load a standard log4net XML file from the file system
        fileViewModel = Log4NetViewModel.LoadFile(filePath);
      }
      else
      {
        // try to load a standard text file from the file system
        fileViewModel = EdiViewModel.LoadFile(filePath);
      }

      if (fileViewModel == null)
      {
        if (this.Config.MruList.FindMRUEntry(filePath) != null)
        {
          if (MsgBox.Msg.Box.Show(string.Format(CultureInfo.CurrentCulture,
                                  "The file:\n\n'{0}'\n\ndoes not exist or cannot be loaded.\n\nDo you want to remove this file from the list of recent files?", filePath),
                                  "Error Loading file", MsgBoxButtons.YesNo) == MsgBoxResult.Yes)
          {
            this.Config.MruList.RemoveEntry(filePath);
          }
        }

        return null;
      }

      this.mFiles.Add(fileViewModel);

      // reset viewmodel options in accordance to current program settings
      EdiViewModel ediVM = fileViewModel as EdiViewModel;

      if (ediVM != null)
        this.SetActiveDocumentOnNewFileOrOpenFile(ref ediVM);
      else
      {
        if (fileViewModel is Log4NetViewModel)
        {
          Log4NetViewModel log4netVM = fileViewModel as Log4NetViewModel;

          this.SetActiveLog4NetDocument(ref log4netVM);
        }
      }

      if (AddIntoMRU == true)
        this.RecentFiles.AddNewEntryIntoMRU(filePath);

      return fileViewModel;
    }

    #region NewCommand
    private void OnNew()
    {
      try
      {
        var vm = new EdiViewModel();

        this.mFiles.Add(vm);
        SetActiveDocumentOnNewFileOrOpenFile(ref vm);
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
      }
    }
    #endregion NewCommand

    #region OpenCommand
    private void OnOpen()
    {
      try
      {
        var dlg = new OpenFileDialog();
        dlg.Filter = Workspace.FileFilter;
        dlg.Multiselect = true;
        dlg.InitialDirectory = this.GetDefaultPath();

        if (dlg.ShowDialog().GetValueOrDefault())
        {
          foreach(string fileName in dlg.FileNames)
          {
            this.Open(fileName);
          }
        }
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Box.Show(exp, App.IssueTrackerTitle, MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
      }
    }
    #endregion Application_Exit_Command

    #region Application_About_Command
    private void AppAbout_CommandExecuted()
    {
      try
      {
        AboutViewModel vm = new AboutViewModel();
        Window dlg = ViewSelector.GetDialogView(vm, Application.Current.MainWindow);

        dlg.ShowDialog();
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
      }
    }
    #endregion Application_About_Command

    #region Recent File List Pin Unpin Commands
    private void PinCommand_Executed(object o, ExecutedRoutedEventArgs e)
    {
      try
      {
        MRUEntryVM cmdParam = o as MRUEntryVM;

        if (cmdParam == null)
          return;

        if (e != null)
          e.Handled = true;

        this.RecentFiles.MruList.PinUnpinEntry(!cmdParam.IsPinned, cmdParam);
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
      }
    }

    private void AddMRUEntry_Executed(object o, ExecutedRoutedEventArgs e)
    {
      try
      {
        MRUEntryVM cmdParam = o as MRUEntryVM;

        if (cmdParam == null)
          return;

        if (e != null)
          e.Handled = true;

        this.RecentFiles.MruList.AddMRUEntry(cmdParam);
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
      }
    }

    private void RemoveMRUEntry_Executed(object o, ExecutedRoutedEventArgs e)
    {
      try
      {
        MRUEntryVM cmdParam = o as MRUEntryVM;

        if (cmdParam == null)
          return;

        if (e != null)
          e.Handled = true;

        this.RecentFiles.MruList.RemovePinEntry(cmdParam);
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
      }
    }
    #endregion // RequestClose [event]

    private void SetActiveLog4NetDocument(ref Log4NetViewModel vm)
    {
      try
      {
        ActiveDocument = vm;
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
      }
    }

    /// <summary>
    /// Reset file view options in accordance with current program settings
    /// whenever a new file is internally created (on File Open or New File)
    /// </summary>
    /// <param name="vm"></param>
    private void SetActiveDocumentOnNewFileOrOpenFile(ref EdiViewModel vm)
    {
      try
      {
        // Set scale factor in default size of text font
        vm.InitScaleView(this.Config.DocumentZoomUnit, this.Config.DocumentZoomView);

        ActiveDocument = vm;
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
          sPath = this.Config.GetLastActivePath();

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
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
      }

      return sPath;
    }

    internal bool OnSave(FileBaseViewModel doc, bool saveAsFlag = false)
    {
      EdiViewModel fileToSave = doc as EdiViewModel;

      if (fileToSave != null && doc != null)
        return this.OnSaveTextFile(fileToSave, saveAsFlag);

      throw new NotSupportedException((doc != null ? doc.ToString() : "Unknown document type"));
    }

    internal bool OnSaveTextFile(EdiViewModel fileToSave, bool saveAsFlag = false)
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
        string sMsg = "An unexpected problem has occurred while saving the file";

        if (filePath.Length > 0)
          sMsg = string.Format(CultureInfo.CurrentCulture, "'{0}'\n" +
                              "has occurred while saving the file\n:'{1}'", Exp.Message, filePath);
        else
          sMsg = string.Format(CultureInfo.CurrentCulture, "'{0}'\n" +
                              "has occurred while saving a file", Exp.Message);

        MsgBox.Msg.Box.Show(sMsg, "An unexpected problem has occurred while saving the file", MsgBoxButtons.OK);
      }

      return false;
    }

    internal bool OnCloseSaveDirtyFile(EdiViewModel fileToClose)
    {
      if (fileToClose.IsDirty)
      {
        var res = MsgBox.Msg.Box.Show(string.Format(CultureInfo.CurrentCulture, "Save changes for file '{0}'?",
                                    fileToClose.FileName), this.ApplicationTitle,
                                    MsgBoxButtons.YesNoCancel,
                                    MsgBoxImage.Question,
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
    /// TODO: The last active document that was active before the document being closed should activated.
    /// </summary>
    /// <param name="fileToClose"></param>
    /// <returns></returns>
    internal bool Close(FileBaseViewModel doc)
    {
      try
      {
        {
          EdiViewModel textFileToClose = doc as EdiViewModel;
          if (textFileToClose != null)
          {
            if (this.OnCloseSaveDirtyFile(textFileToClose) == false)
              return false;

            mFiles.Remove(textFileToClose);

            if (this.Documents.Count == 0)
              this.ActiveDocument = null;
            else
              this.ActiveDocument = this.mFiles[0];

            return true;
          }
        }

        {
          // This could be a start page or a log4net file or any other read-only document
          FileBaseViewModel s = doc as FileBaseViewModel;
          if (s != null)
          {
            mFiles.Remove(doc);

            if (this.Documents.Count == 0)
              this.ActiveDocument = null;
            else
              this.ActiveDocument = this.mFiles[0];

            return true;
          }
        }
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
        try
        {
          App.CreateAppDataFolder();
          this.SerializeLayout(sender);            // Store the current layout for later retrieval
        }
        catch
        {
        }

        if (this.mFiles != null)               // Close all open files and make sure there are no unsaved edits
        {                                     // If there are any: Ask user if edits should be saved
          List<EdiViewModel> l = this.Documents;

          for (int i = 0; i < l.Count; i++)
          {
            EdiViewModel f = l[i];

            if (this.OnCloseSaveDirtyFile(f) == false)
            {
              this.mShutDownInProgress = false;
              return false;               // Cancel shutdown process (return false) if user cancels saving edits
            }
          }
        }
      }
      catch (Exception exp)
      {
        logger.Error(exp.Message, exp);
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
      }

      return true;
    }

    internal void SerializeLayout(object sender)
    {
      XmlLayoutSerializer xmlLayout = null;
      MainWindow mainWin = null;

      if (sender != null)
        mainWin = sender as MainWindow;

      // Create XML Layout, close documents, and save layout if closing went OK
      if (mainWin != null)
      {
        xmlLayout = new XmlLayoutSerializer(mainWin.dockManager);

        xmlLayout.Serialize(System.IO.Path.Combine(App.DirAppData, LayoutFileName));
      }
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
        MsgBox.Msg.Box.Show(exp, "Unhandled Error", MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                            App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);
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
          StartPageViewModel s = new StartPageViewModel();
          this.mFiles.Add(s);

          return s;
        }
      }

      return l[0];
    }

    #endregion methods
  }
}
