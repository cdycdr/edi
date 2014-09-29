namespace EdiDocuments.Module
{
	using System.ComponentModel.Composition;
	using System.Reflection;
	using System.Windows;
	using Edi.Core.Interfaces;
	using Edi.Core.Resources;
	using Edi.Core.View.Pane;
	using EdiDocuments.ViewModels.EdiDoc;
	using EdiDocuments.ViewModels.MiniUml;
	using EdiDocuments.ViewModels.RecentFiles;
	using EdiDocuments.ViewModels.StartPage;
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
	[ModuleExport(typeof(MEFLoadEdiDocuments))]
	public class MEFLoadEdiDocuments : IModule
	{
		#region fields
		private readonly IAvalonDockLayoutViewModel mAvLayout = null;
		private readonly IToolWindowRegistry mToolRegistry = null;
		private readonly ISettingsManager mSettingsManager;
		#endregion fields

		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="avLayout"></param>
		[ImportingConstructor]
		public MEFLoadEdiDocuments(IAvalonDockLayoutViewModel avLayout,
															 IToolWindowRegistry toolRegistry,
															 ISettingsManager settingsManager)
		{
			this.mAvLayout = avLayout;
			this.mToolRegistry = toolRegistry;
			this.mSettingsManager = settingsManager;
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
				this.RegisterStyles(this.mAvLayout.ViewProperties.SelectPanesStyle);
			}

			if (this.mToolRegistry != null)
			{
				this.mToolRegistry.RegisterTool(new RecentFilesViewModel(this.mSettingsManager));
			}
		}

		/// <summary>
		/// Register viewmodel types with <seealso cref="DataTemplate"/> for a view
		/// and return all definitions with a <seealso cref="PanesTemplateSelector"/> instance.
		/// </summary>
		/// <param name="paneSel"></param>
		/// <returns></returns>
		private PanesTemplateSelector RegisterDataTemplates(PanesTemplateSelector paneSel)
		{
			// StartPageView
			var template = ResourceLocator.GetResource<DataTemplate>(
									Assembly.GetAssembly(typeof(StartPageViewModel)).GetName().Name,
									"DataTemplates/StartPageViewDataTemplate.xaml",
									"StartPageViewDataTemplate") as DataTemplate;

			paneSel.RegisterDataTemplate(typeof(StartPageViewModel), template);

			//EdiView
			template = ResourceLocator.GetResource<DataTemplate>(
									Assembly.GetAssembly(typeof(EdiViewModel)).GetName().Name,
									"DataTemplates/EdiViewDataTemplate.xaml",
									"EdiViewDataTemplate") as DataTemplate;

			paneSel.RegisterDataTemplate(typeof(EdiViewModel), template);

			// MiniUml
			template = ResourceLocator.GetResource<DataTemplate>(
									Assembly.GetAssembly(typeof(MiniUmlViewModel)).GetName().Name,
									"DataTemplates/MiniUMLViewDataTemplate.xaml",
									"MiniUMLViewDataTemplate") as DataTemplate;

			paneSel.RegisterDataTemplate(typeof(MiniUmlViewModel), template);

			// RecentFiles
			template = ResourceLocator.GetResource<DataTemplate>(
									Assembly.GetAssembly(typeof(RecentFilesViewModel)).GetName().Name,
									"DataTemplates/RecentFilesViewDataTemplate.xaml",
									"RecentFilesViewDataTemplate") as DataTemplate;

			paneSel.RegisterDataTemplate(typeof(RecentFilesViewModel), template);

			return paneSel;
		}

		private PanesStyleSelector RegisterStyles(PanesStyleSelector selectPanesStyle)
		{
			var newStyle = ResourceLocator.GetResource<Style>(
									"EdiApp",
									"Resources/Styles/AvalonDockStyles.xaml",
									"StartPageStyle") as Style;

			selectPanesStyle.RegisterStyle(typeof(StartPageViewModel), newStyle);

			return selectPanesStyle;
		}
		#endregion methods
	}
}
