﻿namespace EdiTools.Module
{
	using System.ComponentModel.Composition;
	using System.Reflection;
	using System.Windows;
	using Edi.Core.Interfaces;
	using Edi.Core.Resources;
	using Edi.Core.View.Pane;
	using EdiTools.ViewModels.FileExplorer;
	using EdiTools.ViewModels.FileStats;
	using FileSystemModels.Models;
	using Microsoft.Practices.Prism.MefExtensions.Modularity;
	using Microsoft.Practices.Prism.Modularity;
	using Settings.Interfaces;

	/// <summary>
	/// PRISM MEF Loader/Initializer class
	/// Relevante services are injected in constructor.
	/// 
	/// Requires the following XML in App.config to enable PRISM MEF to pick-up contained definitions.
	/// 
	/// &lt;modules>
	/// &lt;!-- Edi.Tools assembly from plugins folder and initialize it if it was present -->
	/// &lt;module assemblyFile="EdiTools.dll"
	/// 				moduleType="EdiTools.Module.Loader, EdiTools.Module.Loader, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
	/// 				moduleName="Loader"
	/// 				startupLoaded="true" />
	/// &lt;/modules>
	/// </summary>
	[ModuleExport(typeof(MEFLoadEdiTools), InitializationMode = InitializationMode.WhenAvailable)]
	public class MEFLoadEdiTools : IModule
	{
		#region fields
		private readonly IAvalonDockLayoutViewModel mAvLayout;
		private readonly IToolWindowRegistry mToolRegistry;
		private readonly ISettingsManager mSettingsManager;
		private readonly IFileOpenService mFileOpenService;
		#endregion fields

		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="avLayout"></param>
		[ImportingConstructor]
		public MEFLoadEdiTools(IAvalonDockLayoutViewModel avLayout,
													 ISettingsManager programSettings,
													 IToolWindowRegistry toolRegistry,
													 IFileOpenService fileOpenService)
		{
			this.mAvLayout = avLayout;
			this.mSettingsManager = programSettings;
			this.mToolRegistry = toolRegistry;
			this.mFileOpenService = fileOpenService;
		}

		#region methods
		/// <summary>
		/// Initialize this module via standard PRISM MEF procedure
		/// </summary>
		void IModule.Initialize()
		{
			if (this.mAvLayout != null)
			{
				this.RegisterDataTemplates(this.mAvLayout.ViewProperties.SelectPanesTemplate);
			}

			if (this.mToolRegistry != null)
			{
				this.mToolRegistry.RegisterTool(new FileStatsViewModel());
				RegisterFileExplorerViewModel();
			}
		}

		private void RegisterFileExplorerViewModel()
		{
			var FileExplorer = new FileExplorerViewModel(this.mSettingsManager, this.mFileOpenService);

			ExplorerSettingsModel settings = null;

			settings = this.mSettingsManager.SettingData.ExplorerSettings;

			if (settings == null)
				settings = new ExplorerSettingsModel();

			settings.UserProfile = this.mSettingsManager.SessionData.LastActiveExplorer;

			// (re-)configure previous explorer settings and (re-)activate current location
			FileExplorer.Settings.ConfigureExplorerSettings(settings);

			this.mToolRegistry.RegisterTool(FileExplorer);
		}

		/// <summary>
		/// Register viewmodel types with <seealso cref="DataTemplate"/> for a view
		/// and return all definitions with a <seealso cref="PanesTemplateSelector"/> instance.
		/// </summary>
		/// <param name="paneSel"></param>
		/// <returns></returns>
		private PanesTemplateSelector RegisterDataTemplates(PanesTemplateSelector paneSel)
		{
			// FileStatsView
			var template = ResourceLocator.GetResource<DataTemplate>(
											Assembly.GetAssembly(typeof(FileStatsViewModel)).GetName().Name,
											"DataTemplates/FileStatsViewDataTemplate.xaml",
											"FileStatsViewTemplate") as DataTemplate;

			paneSel.RegisterDataTemplate(typeof(FileStatsViewModel), template);

			// FileExplorer
			template = ResourceLocator.GetResource<DataTemplate>(
									Assembly.GetAssembly(typeof(FileExplorerViewModel)).GetName().Name,
									"DataTemplates/FileExplorerViewDataTemplate.xaml",
									"FileExplorerViewDataTemplate") as DataTemplate;

			paneSel.RegisterDataTemplate(typeof(FileExplorerViewModel), template);

			return paneSel;
		}
		#endregion methods
	}
}
