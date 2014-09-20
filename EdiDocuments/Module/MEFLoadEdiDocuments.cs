namespace EdiDocuments.Module
{
	using System.ComponentModel.Composition;
	using System.Reflection;
	using System.Windows;
	using Edi.Core.Interfaces;
	using Edi.Core.Resources;
	using Edi.Core.View.Pane;
	using EdiDocuments.ViewModels.EdiDoc;
	using EdiDocuments.ViewModels.Log4Net;
	using EdiDocuments.ViewModels.MiniUml;
	using EdiDocuments.ViewModels.RecentFiles;
	using EdiDocuments.ViewModels.StartPage;
	using Microsoft.Practices.Prism.MefExtensions.Modularity;
	using Microsoft.Practices.Prism.Modularity;

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
		IAvalonDockLayoutViewModel mAvLayout = null;

		/// <summary>
		/// Class constructor
		/// </summary>
		/// <param name="avLayout"></param>
		[ImportingConstructor]
		public MEFLoadEdiDocuments(IAvalonDockLayoutViewModel avLayout)
		{
			this.mAvLayout = avLayout;
		}

		#region methods
		void IModule.Initialize()
		{
			this.RegisterDataTemplates(this.mAvLayout.ViewProperties.SelectPanesTemplate);
			this.RegisterStyles(this.mAvLayout.ViewProperties.SelectPanesStyle);
		}

		/// <summary>
		/// Register viewmodel types with <seealso cref="DataTemplate"/> for a view
		/// and return all definitions with a <seealso cref="PanesTemplateSelector"/> instance.
		/// </summary>
		/// <param name="paneSel"></param>
		/// <returns></returns>
		private PanesTemplateSelector RegisterDataTemplates(PanesTemplateSelector paneSel)
		{
			/* FileStatsView
			var template = ResourceLocator.GetResource<DataTemplate>(
											Assembly.GetAssembly(typeof(FileStatsViewModel)).GetName().Name,
											"DataTemplates/FileStatsViewDataTemplate.xaml",
											"FileStatsViewTemplate") as DataTemplate;

			paneSel.RegisterDataTemplate(typeof(FileStatsViewModel), template);
*/

			// StartPageView
			var template = ResourceLocator.GetResource<DataTemplate>(
									Assembly.GetAssembly(typeof(StartPageViewModel)).GetName().Name,
									"DataTemplates/StartPageViewDataTemplate.xaml",
									"StartPageViewDataTemplate") as DataTemplate;

			paneSel.RegisterDataTemplate(typeof(StartPageViewModel), template);

			// Register Log4Net DataTemplates     
			template = ResourceLocator.GetResource<DataTemplate>(
									Assembly.GetAssembly(typeof(Log4NetViewModel)).GetName().Name,
									"DataTemplates/Log4NetViewDataTemplate.xaml",
									"Log4NetDocViewDataTemplate") as DataTemplate;

			paneSel.RegisterDataTemplate(typeof(Log4NetViewModel), template);

			template = ResourceLocator.GetResource<DataTemplate>(
									Assembly.GetAssembly(typeof(Log4NetMessageToolViewModel)).GetName().Name,
									"DataTemplates/Log4NetViewDataTemplate.xaml",
									"Log4NetMessageViewDataTemplate") as DataTemplate;

			paneSel.RegisterDataTemplate(typeof(Log4NetMessageToolViewModel), template);

			template = ResourceLocator.GetResource<DataTemplate>(
									Assembly.GetAssembly(typeof(Log4NetToolViewModel)).GetName().Name,
									"DataTemplates/Log4NetViewDataTemplate.xaml",
									"Log4NetToolViewDataTemplate") as DataTemplate;

			paneSel.RegisterDataTemplate(typeof(Log4NetToolViewModel), template);

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

			/* FileExplorer
			template = ResourceLocator.GetResource<DataTemplate>(
									Assembly.GetAssembly(typeof(FileExplorerViewModel)).GetName().Name,
									"DataTemplates/FileExplorerViewDataTemplate.xaml",
									"FileExplorerViewDataTemplate") as DataTemplate;

			paneSel.RegisterDataTemplate(typeof(FileExplorerViewModel), template);
*/
			return paneSel;
		}

		private PanesStyleSelector RegisterStyles(PanesStyleSelector selectPanesStyle)
		{
			var newStyle = ResourceLocator.GetResource<Style>(
									"EdiApp",
									"Resources/Styles/AvalonDockStyles.xaml",
									"StartPageStyle") as Style;

			selectPanesStyle.RegisterStyle(typeof(StartPageViewModel), newStyle);

			newStyle = ResourceLocator.GetResource<Style>(
									"EdiApp",
									"Resources/Styles/AvalonDockStyles.xaml",
									"Log4NetStyle") as Style;

			selectPanesStyle.RegisterStyle(typeof(Log4NetViewModel), newStyle);

			return selectPanesStyle;
		}
		#endregion methods
	}
}
