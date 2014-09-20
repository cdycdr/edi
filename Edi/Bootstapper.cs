namespace Edi
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Composition.Hosting;
	using System.Windows;
	using Edi.Core.Interfaces;
	using EdiApp.ViewModels;
	using EdiDocuments.ViewModels.StartPage;
	using EdiTools.ViewModels.FileStats;
	using EdiViews.ViewModel;
	using Microsoft.Practices.Prism.MefExtensions;
	using Microsoft.Practices.Prism.Modularity;
	using MsgBox;
	using Settings;
	using Settings.ProgramSettings;
	using SimpleControls.Local;

	public class Bootstapper : MefBootstrapper
	{
		#region fields
		protected static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private MainWindow mMainWin = null;
		private IApplicationViewModel appVM = null;

		private StartupEventArgs mEventArgs;
		private App mApp = null;
		private readonly Options mProgramSettings = null;
		#endregion fields

		#region constructors
		public Bootstapper(App app, StartupEventArgs eventArgs, Options programSettings)
		{
			this.mEventArgs = eventArgs;
			this.mApp = app;
			this.mProgramSettings = programSettings;
		}
		#endregion constructors

		#region properties
		private IApplicationViewModel AppViewModel
		{
			get
			{
				return this.appVM;
			}
		}

		private MainWindow MainWindow
		{
			get
			{
				return this.MainWindow;
			}
		}
		#endregion properties

		#region Methods
		protected override void ConfigureAggregateCatalog()
		{
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(IAppCoreModel).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(AvalonDockLayoutViewModel).Assembly));
			this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(Bootstapper).Assembly));

			////this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(FileStatsViewModel).Assembly));
			////this.AggregateCatalog.Catalogs.Add(new AssemblyCatalog(typeof(StartPageViewModel).Assembly));
		}

		protected override DependencyObject CreateShell()
		{
			try
			{
				var appCore = this.Container.GetExportedValue<IAppCoreModel>();

				var settings = SettingsManager.Instance;

				// Setup localtion of config files
				settings.AppDir = appCore.DirAppData;
				settings.LayoutFileName = appCore.LayoutFileName;

				var avLayout = this.Container.GetExportedValue<IAvalonDockLayoutViewModel>();
				this.appVM = this.Container.GetExportedValue<IApplicationViewModel>();

				appVM.LoadConfigOnAppStartup(this.mProgramSettings);

				// Attempt to load a MiniUML plugin via the model class
				MiniUML.Model.MiniUmlPluginLoader.LoadPlugins(appCore.AssemblyEntryLocation + @"\MiniUML.Plugins\", this.AppViewModel);

				this.mMainWin = this.Container.GetExportedValue<MainWindow>();

				appCore.CreateAppDataFolder();

				if (this.mMainWin != null)
				{
					this.ConstructMainWindowSession(this.appVM, this.mMainWin);

					this.mApp.ShutdownMode = System.Windows.ShutdownMode.OnLastWindowClose;
					////this.mMainWin.Show();

					if (this.mEventArgs != null)
						ProcessCmdLine(this.mEventArgs.Args, this.appVM);
				}
				else
					throw new Exception("Main Window construction failed in application boot strapper class.");
			}
			catch (Exception exp)
			{
				logger.Error(exp);
				this.mApp.ShutdownMode = ShutdownMode.OnExplicitShutdown;
				Application.Current.MainWindow = null;

				MsgBox.Msg.Show(exp, Strings.STR_MSG_ERROR_FINDING_RESOURCE, MsgBoxButtons.OKCopy);

				this.mApp.Shutdown();
			}

			return this.mMainWin;
		}

		protected override void InitializeShell()
		{
			base.InitializeShell();

			Application.Current.MainWindow = (MainWindow)this.Shell;
			Application.Current.MainWindow.Show();
		}

		protected override Microsoft.Practices.Prism.Regions.IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
		{
			var factory = base.ConfigureDefaultRegionBehaviors();
			return factory;
		}

		/// <summary>
		/// Creates the <see cref="IModuleCatalog"/> used by Prism.
		/// 
		/// The base implementation returns a new ModuleCatalog.
		/// </summary>
		/// <returns>
		/// A ConfigurationModuleCatalog.
		/// </returns>
		protected override IModuleCatalog CreateModuleCatalog()
		{
			// Configure Prism ModuleCatalog via app.config configuration file
			return new ConfigurationModuleCatalog();
		}
		#endregion Methods

		#region shell handling methods
		/// <summary>
		/// Interpret command line parameters and process their content
		/// </summary>
		/// <param name="args"></param>
		private static void ProcessCmdLine(IEnumerable<string> args, IApplicationViewModel appVM)
		{
			if (args != null)
			{
				logger.InfoFormat("TRACE Processing command line 'args' in App.ProcessCmdLine");

				foreach (string sPath in args)
				{
					logger.InfoFormat("TRACE Processing CMD param: '{0}'", sPath);

					// Command may not be bound yet so we do this via direct call
					appVM.Open(sPath);
				}
			}
			else
				logger.InfoFormat("TRACE There are no command line 'args' to process in App.ProcessCmdLine");
		}

		/// <summary>
		/// COnstruct MainWindow an attach datacontext to it.
		/// </summary>
		/// <param name="workSpace"></param>
		/// <param name="win"></param>
		private void ConstructMainWindowSession(IApplicationViewModel workSpace, Window win)
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

				// Initialize Window State in viewmodel to show resize grip when window is not maximized
				if (win.WindowState == WindowState.Maximized)
					workSpace.IsNotMaximized = false;
				else
					workSpace.IsNotMaximized = true;

				string lastActiveFile = SettingsManager.Instance.SessionData.LastActiveFile;

				MainWindow mainWin = win as MainWindow;

				// if (mainWin != null)
				//  mainWin.Loaded += App.MainWindow_Loaded;
			}
			catch
			{
				throw;
			}
		}
		#endregion shell handling methods
	}
}
