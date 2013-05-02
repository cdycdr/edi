namespace Edi
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Reflection;
  using System.Threading;
  using System.Windows;
  using System.Windows.Threading;
  using AvalonDock.Layout.Serialization;
  using Edi.ViewModel;
  using EdiViews.Config.ViewModel;
  using EdiViews.Documents.StartPage;
  using EdiViews.FileStats;
  using EdiViews.Log4Net;
  using EdiViews.ViewModel;
  using log4net;
  using log4net.Config;
  using MsgBox;
  using Util;
  using Util.ActivateWindow;

  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    #region fields
    readonly SingletonApplicationEnforcer enforcer = new SingletonApplicationEnforcer(ProcessSecondInstance, WindowLister.ActivateMainWindow, "Edi");

    protected static readonly log4net.ILog logger;

    static App()
    {
      XmlConfigurator.Configure();
      logger = LogManager.GetLogger("default");
    }

    private Window mMainWin;

    public const string IssueTrackerTitle = "Unhandled Error";
    public const string IssueTrackerText = "Please click on the link below to check if this is a known problem.\n\n" +
                                           "Please report this problem if it is not known, yet.\n\n" +
                                           "The problem report should contain the error message below (you can use the copy button)\n" +
                                           "and a statement about the function/workflow you were using. Attach screenshots or sample files if applicable.";
    public const string IssueTrackerLink = "https://edi.codeplex.com/workitem/list/basic";
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
                                      string.Format(CultureInfo.InvariantCulture, "{0}.session", App.AssemblyTitle));
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
        logger.Error(exp);
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
        logger.InfoFormat("TRACE Processing command line 'args' in App.ProcessCmdLine");

        foreach (string sPath in args)
        {
          logger.InfoFormat("TRACE Processing CMD param: '{0}'", sPath);

          // Command may not be bound yet so we do this via direct call
          Workspace.This.Open(sPath);
        }
      }
      else
        logger.InfoFormat("TRACE There are no command line 'args' to process in App.ProcessCmdLine");
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
          logger.Error(string.Format(CultureInfo.InvariantCulture,
                       "The {0} application received request to shutdown: {1}.\n\nPlease save your data and re-start manually.",
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
        logger.Error(exp);
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
      }
      catch
      {
      }

      try
      {
        Workspace.This.LoadConfigOnAppStartup();

        if (Workspace.This.Config.RunSingleInstance == true)
        {
          if (enforcer.ShouldApplicationExit() == true)
            this.Shutdown();
        }
      }
      catch
      {
      }

      try
      {
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

          this.LoadSession(Workspace.This, this.mMainWin);
          this.mMainWin.Show();

          if (e != null)
            ProcessCmdLine(e.Args);
        }
      }
      catch (Exception exp)
      {
        logger.Error(exp);
      }
    }

    private void LoadSession(Workspace workSpace, Window win)
    {
      try
      {
        win.DataContext = workSpace;

        // Establish command binding to accept user input via commanding framework
        workSpace.InitCommandBinding(win);

        workSpace.Config.MainWindowPosSz.SetPos(win);      // (Re)-set mainWindow view coordinates

        string lastActiveFile = workSpace.Config.LastActiveFile;

        MainWindow mainWin = win as MainWindow;

        if (mainWin != null)
          mainWin.Loaded += App.MainWindow_Loaded;
      }
      catch (Exception exp)
      {
        logger.Error(exp);
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

      string lastActiveFile = Workspace.This.Config.LastActiveFile;
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
              if (Workspace.This.Config.ReloadOpenFilesOnAppStart == true)
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
        logger.Error(exp);
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
        Console.WriteLine("  >>> Shuting down application.");

        // Persist window position, width and height from this session
        appVM.Config.MainWindowPosSz = new ViewPosSzViewModel(win);

        // Save/initialize program options that determine global programm behaviour
        appVM.SaveConfigOnAppClosed();
      }
      catch (Exception exp)
      {
        logger.Error(exp);
        Msg.Box.Show(exp.ToString(), "Error in shut-down process", MsgBoxButtons.OK, MsgBoxImage.Error);
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
          message = "An unknown error occurred.";

        logger.Error(message);

        Msg.Box.Show(e.Exception, "Unhandled Error",
                      MsgBoxButtons.OK, MsgBoxImage.Error, MsgBoxResult.NoDefaultButton,
                      App.IssueTrackerLink, App.IssueTrackerLink, App.IssueTrackerText, null, true);

        e.Handled = true;
      }
      catch (Exception exp)
      {
        logger.Error("An error occured while dispatching unhandled exception", exp);
      }
    }
    #endregion methods
  }
}
