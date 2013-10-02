namespace Edi
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Reflection;
  using System.Threading;
  using System.Windows;
  using System.Windows.Threading;
  using Edi.ViewModel;
  using EdiViews.Config.ViewModel;
  using EdiViews.Documents.StartPage;
  using EdiViews.FileStats;
  using EdiViews.Log4Net;
  using EdiViews.ViewModel;
  using log4net;
  using log4net.Config;
  using MsgBox;
  using Settings;
  using Settings.UserProfile;
  using Util;
  using Util.ActivateWindow;
  using Xceed.Wpf.AvalonDock.Layout.Serialization;

  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    #region fields
    readonly SingletonApplicationEnforcer enforcer = new SingletonApplicationEnforcer(ProcessSecondInstance, WindowLister.ActivateMainWindow, "Edi");

    protected static log4net.ILog Logger;

    static App()
    {
      XmlConfigurator.Configure();
      Logger = LogManager.GetLogger("default");
    }

    private Window mMainWin;

    public static readonly string IssueTrackerTitle = Util.Local.Strings.STR_MSG_IssueTrackerTitle;
    public static readonly string IssueTrackerText = Util.Local.Strings.STR_MSG_IssueTrackerText;
    public const string IssueTrackerLink  = "https://edi.codeplex.com/workitem/list/basic";
    #endregion fields

    #region constructor
    public App()
    {
      this.InitializeComponent(); 
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get a path to the directory where the application
    /// can persist/load user data on session exit and re-start.
    /// </summary>
    public static string DirAppData
    {
      get
      {
        return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) +
                                         System.IO.Path.DirectorySeparatorChar +
                                         App.Company;
      }
    }

    /// <summary>
    /// Get a path to the directory where the user store his documents
    /// </summary>
    public static string MyDocumentsUserDir
    {
      get
      {
        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
      }
    }

    /// <summary>
    /// Get the name of the executing assembly (usually name of *.exe file)
    /// </summary>
    internal static string AssemblyTitle
    {
      get
      {
        return Assembly.GetEntryAssembly().GetName().Name;
      }
    }

    //
    // Summary:
    //     Gets the path or UNC location of the loaded file that contains the manifest.
    //
    // Returns:
    //     The location of the loaded file that contains the manifest. If the loaded
    //     file was shadow-copied, the location is that of the file after being shadow-copied.
    //     If the assembly is loaded from a byte array, such as when using the System.Reflection.Assembly.Load(System.Byte[])
    //     method overload, the value returned is an empty string ("").
    internal static string AssemblyEntryLocation
    {
      get
      {
        return System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
      }
    }

    internal static string Company
    {
      get
      {
        return "Edi";
      }
    }

    /// <summary>
    /// Get path and file name to application specific session file
    /// </summary>
    internal static string DirFileAppSessionData
    {
      get
      {
        return System.IO.Path.Combine(App.DirAppData,
                                      string.Format(CultureInfo.InvariantCulture, "{0}.App.session", App.AssemblyTitle));
      }
    }

    /// <summary>
    /// Get path and file name to application specific settings file
    /// </summary>
    internal static string DirFileAppSettingsData
    {
      get
      {
        return System.IO.Path.Combine(App.DirAppData,
                                      string.Format(CultureInfo.InvariantCulture, "{0}.App.settings", App.AssemblyTitle));
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Create a dedicated directory to store program settings and session data
    /// </summary>
    /// <returns></returns>
    public static bool CreateAppDataFolder()
    {
      try
      {
        if (System.IO.Directory.Exists(App.DirAppData) == false)
          System.IO.Directory.CreateDirectory(App.DirAppData);
      }
      catch (Exception exp)
      {
        Logger.Error(exp);
        return false;
      }

      return true;
    }

    /// <summary>
    /// Interpret command line parameters and process their content
    /// </summary>
    /// <param name="args"></param>
    private static void ProcessCmdLine(IEnumerable<string> args)
    {
      if (args != null)
      {
        Logger.InfoFormat("TRACE Processing command line 'args' in App.ProcessCmdLine");

        foreach (string sPath in args)
        {
          Logger.InfoFormat("TRACE Processing CMD param: '{0}'", sPath);

          // Command may not be bound yet so we do this via direct call
          Workspace.This.Open(sPath);
        }
      }
      else
        Logger.InfoFormat("TRACE There are no command line 'args' to process in App.ProcessCmdLine");
    }

    /// <summary>
    /// Process command line args and window activation when switching from 1st to 2nd instance
    /// </summary>
    /// <param name="args"></param>
    public static void ProcessSecondInstance(IEnumerable<string> args)
    {
      var dispatcher = Current.Dispatcher;
      if (dispatcher.CheckAccess())
      {
        // The current application is the first
        // This case is already handled via start-up code in App.cs
        // ShowArgs(args);
      }
      else
      {
        dispatcher.BeginInvoke(
          new Action(delegate
          {
            App.RestoreCurrentMainWindow();

            var mainWindow = Current.MainWindow as MainWindow;

            if (mainWindow != null)
            {
              if (mainWindow.IsVisible == false)
                mainWindow.Show();

              if (mainWindow.WindowState == WindowState.Minimized)
                mainWindow.WindowState = WindowState.Normal;

              mainWindow.Topmost = true;
              //mainWindow.Show();

              Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (Action)delegate
              {
                bool bActive = mainWindow.Activate();

                mainWindow.Topmost = false;
              });
            }

            // Filter name of executeable if present in command line args
            if (args != null)
              ProcessCmdLine(App.FilterAssemblyName(args));

          }));
      }
    }

    /// <summary>
    /// Restore the applications window from minimized state
    /// into non-minimzed state and send it to the top to make
    /// sure its visible for the user.
    /// </summary>
    public static void RestoreCurrentMainWindow()
    {
      if (Application.Current != null)
      {
        if (Application.Current.MainWindow != null)
        {
          Window win = Application.Current.MainWindow;

          if (win.IsVisible == false)
            win.Show();

          if (win.WindowState == WindowState.Minimized)
            win.WindowState = WindowState.Normal;

          win.Topmost = true;
          win.Topmost = false;
        }
      }
    }

    /// <summary>
    /// Check if end of application session should be canceled or not
    /// (we may have gotten here through unhandled exceptions - so we
    /// display it and attempt CONTINUE so user can save his data.
    /// </summary>
    /// <param name="e"></param>
    protected override void OnSessionEnding(SessionEndingCancelEventArgs e)
    {
      base.OnSessionEnding(e);

      try
      {
        try
        {
          Logger.Error(string.Format(CultureInfo.InvariantCulture,
                       "The {0} application received request to shutdown: {1}.",
                       Application.ResourceAssembly.GetName(), e.ReasonSessionEnding.ToString()));
        }
        catch
        {
        }

        if (this.mMainWin != null)
        {
          if (this.mMainWin.DataContext != null && Workspace.This.Files != null)
          {
            // Close all open files and check whether application is ready to close
            if (Workspace.This.Exit_CheckConditions(this.mMainWin) == true)
              e.Cancel = false;
            else
              e.Cancel = Workspace.This.ShutDownInProgress_Cancel = true;
          }
        }
      }
      catch (Exception exp)
      {
        Logger.Error(exp);
      }
    }

    /// <summary>
    /// Filter name of executeable if present in command line args
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    private static List<string> FilterAssemblyName(IEnumerable<string> args)
    {
      List<string> filterCmdLineArgs = new List<string>();

      if (args != null)
      {
        int Cnt = 0;
        foreach (string s in args)
        {
          Cnt++;

          if (Cnt == 1)  // Always remove first command line parameter
            continue;    // since this is the assembly entry name (Edi.exe)

          filterCmdLineArgs.Add(s);
        }
      }

      return filterCmdLineArgs;
    }

    /// <summary>
    /// This is the first bit of code being executed when the application is invoked (main entry point).
    /// 
    /// Use the <paramref name="e"/> parameter to evaluate command line options.
    /// Invoking a program with an associated file type extension (eg.: *.txt) in Explorer
    /// results, for example, in executing this function with the path and filename being
    /// supplied in <paramref name="e"/>.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Application_Startup(object sender, StartupEventArgs e)
    {
      try
      {
        // Set shutdown mode here (and reset further below) to enable showing custom dialogs (messageboxes)
        // durring start-up without shutting down application when the custom dialogs (messagebox) closes
        this.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;

        Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
        //Thread.CurrentThread.CurrentCulture = new CultureInfo("de-DE");
        //Thread.CurrentThread.CurrentUICulture = new CultureInfo("de-DE");
      }
      catch
      {
      }

      try
      {
        Workspace.This.LoadConfigOnAppStartup();

        if (SettingsManager.Instance.SettingData.RunSingleInstance == true)
        {
          if (enforcer.ShouldApplicationExit() == true)
            this.Shutdown();
        }
      }
      catch (Exception exp)
      {
        Logger.Error(exp);
      }

      try
      {
        MiniUmlPluginLoader.LoadPlugins(App.AssemblyEntryLocation + @"\MiniUML.Plugins\", Workspace.This);
        
        Application.Current.MainWindow = this.mMainWin = new MainWindow();
        this.ShutdownMode = System.Windows.ShutdownMode.OnLastWindowClose;

        App.CreateAppDataFolder();

        if (this.mMainWin != null)
        {
          this.mMainWin.Closing += this.OnClosing;

          // When the ViewModel asks to be closed, close the window.
          // Source: http://msdn.microsoft.com/en-us/magazine/dd419663.aspx
          Workspace.This.RequestClose += delegate
          {
            // Save session data and close application
            this.OnClosed(this.mMainWin.DataContext as ViewModel.Workspace, this.mMainWin);
          };

          this.ConstructMainWindowSession(Workspace.This, this.mMainWin);
          this.mMainWin.Show();

          if (e != null)
            ProcessCmdLine(e.Args);
        }
      }
      catch (Exception exp)
      {
        Logger.Error(exp);
      }
    }

    /// <summary>
    /// COnstruct MainWindow an attach datacontext to it.
    /// </summary>
    /// <param name="workSpace"></param>
    /// <param name="win"></param>
    private void ConstructMainWindowSession(Workspace workSpace, Window win)
    {
      try
      {
        win.DataContext = workSpace;

        // Establish command binding to accept user input via commanding framework
        workSpace.InitCommandBinding(win);

        win.Left = SettingsManager.Instance.SessionData.MainWindowPosSz.X;
        win.Top = SettingsManager.Instance.SessionData.MainWindowPosSz.Y;
        win.Width = SettingsManager.Instance.SessionData.MainWindowPosSz.Width;
        win.Height = SettingsManager.Instance.SessionData.MainWindowPosSz.Height;
        win.WindowState = (SettingsManager.Instance.SessionData.MainWindowPosSz.IsMaximized == true ? WindowState.Maximized : WindowState.Normal);

        string lastActiveFile = SettingsManager.Instance.SessionData.LastActiveFile;

        MainWindow mainWin = win as MainWindow;

        if (mainWin != null)
          mainWin.Loaded += App.MainWindow_Loaded;
      }
      catch (Exception exp)
      {
        Logger.Error(exp);
      }
    }

    /// <summary>
    /// Re-load dockument tab and tool window layout.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private static void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
      MainWindow win = sender as MainWindow;

      if (win == null)
        return;

      string lastActiveFile = SettingsManager.Instance.SessionData.LastActiveFile;
      string layoutFileName = System.IO.Path.Combine(App.DirAppData, "Layout.config");

      if (System.IO.File.Exists(layoutFileName) == true)
      {
        var layoutSerializer = new XmlLayoutSerializer(win.dockManager);

        layoutSerializer.LayoutSerializationCallback += (s, args) =>
        {
          string sId = args.Model.ContentId;

          // Empty Ids ar einvalide but possible if aaplication is closed with File>New without edits.
          if (string.IsNullOrWhiteSpace(sId) == true)
          {
            args.Cancel = true;
            return;
          }

          if (args.Model.ContentId == FileStatsViewModel.ToolContentId)
            args.Content = Workspace.This.FileStats;
          else
            if (args.Model.ContentId == RecentFilesViewModel.ToolContentId)
              args.Content = (object)Workspace.This.RecentFiles;
            else
            if (args.Model.ContentId == Log4NetToolViewModel.ToolContentId)
              args.Content = (object)Workspace.This.Log4NetTool; // Re-create log4net tool window binding
            else
            if (args.Model.ContentId == Log4NetMessageToolViewModel.ToolContentId)
              args.Content = (object)Workspace.This.Log4NetMessageTool; // Re-create log4net message tool window binding
            else
            {
              if (SettingsManager.Instance.SettingData.ReloadOpenFilesOnAppStart == true)
              {
                if (!string.IsNullOrWhiteSpace(args.Model.ContentId))
                {
                  switch (args.Model.ContentId)
                  {
                    case StartPageViewModel.StartPageContentId: // Re-create start page content
                      if (Workspace.This.GetStartPage(false) == null)
                      {
                        args.Content = Workspace.This.GetStartPage(true);
                      }
                      break;
                    
                    default:
                      if (System.IO.File.Exists(args.Model.ContentId))
                      {
                        // Re-create Edi document (text file or log4net document) content content
                        args.Content = Workspace.This.Open(args.Model.ContentId);
                      }
                      else
                        args.Cancel = true;
                      break;
                  }
                }
                else
                  args.Cancel = true;
              }
            }
        };

        layoutSerializer.Deserialize(layoutFileName);

        Workspace.This.SetActiveDocument(lastActiveFile);
      }
    }

    /// <summary>
    /// Save session data on closing
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
    {
      try
      {
        Workspace wsVM = Workspace.This;

        if (wsVM != null)
        {
          if (wsVM.Exit_CheckConditions(sender) == true)      // Close all open files and check whether application is ready to close
          {
            wsVM.OnRequestClose();                          // (other than exception and error handling)

            e.Cancel = false;
            //if (wsVM != null)
            //  wsVM.SaveConfigOnAppClosed(); // Save application layout
          }
          else
            e.Cancel = wsVM.ShutDownInProgress_Cancel = true;
        }
      }
      catch (Exception exp)
      {
        Logger.Error(exp);
      }
    }

    /// <summary>
    /// Execute closing function and persist session data to be reloaded on next restart
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnClosed(ViewModel.Workspace appVM, Window win)
    {
      try
      {
        // Persist window position, width and height from this session
        SettingsManager.Instance.SessionData.MainWindowPosSz =
          new ViewPosSizeModel(win.Left, win.Top, win.Width, win.Height,
                               (win.WindowState == WindowState.Maximized ? true : false));

        // Save/initialize program options that determine global programm behaviour
        appVM.SaveConfigOnAppClosed();
      }
      catch (Exception exp)
      {
        Logger.Error(exp);
        Msg.Show(exp.ToString(), Util.Local.Strings.STR_MSG_UnknownError_InShutDownProcess, MsgBoxButtons.OK, MsgBoxImage.Error);
      }
    }

    /// <summary>
    /// Handle unhandled exception here
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
      string message = string.Empty;

      try
      {
        if (e.Exception != null)
        {
          message = string.Format(CultureInfo.CurrentCulture, "{0}\n\n{1}", e.Exception.Message, e.Exception.ToString());
        }
        else
          message = Util.Local.Strings.STR_Msg_UnknownError;

        Logger.Error(message);

        Msg.Show(e.Exception, Util.Local.Strings.STR_MSG_UnknownError_Caption,
                  MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                  App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);

        e.Handled = true;
      }
      catch (Exception exp)
      {
        Logger.Error(Util.Local.Strings.STR_MSG_UnknownError_InErrorDispatcher, exp);
      }
    }
    #endregion methods
  }
}
