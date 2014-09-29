namespace Log4NetTools.Module
{
	using System.ComponentModel.Composition;
	using System.Reflection;
	using System.Windows;
	using Edi.Core.Interfaces;
	using Edi.Core.Resources;
	using Edi.Core.View.Pane;
	using Log4NetTools.ViewModels;
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
	[ModuleExport(typeof(MEFLoadLog4NetTools))]
	public class MEFLoadLog4NetTools : IModule
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
		public MEFLoadLog4NetTools(IAvalonDockLayoutViewModel avLayout,
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
				this.mToolRegistry.RegisterTool(new Log4NetMessageToolViewModel());
				this.mToolRegistry.RegisterTool(new Log4NetToolViewModel());
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
			// Register Log4Net DataTemplates
			var template = ResourceLocator.GetResource<DataTemplate>(
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

			return paneSel;
		}

		private PanesStyleSelector RegisterStyles(PanesStyleSelector selectPanesStyle)
		{
			var newStyle = ResourceLocator.GetResource<Style>(
									"Log4NetTools", "Styles/AvalonDockStyles.xaml", "Log4NetStyle") as Style;

			selectPanesStyle.RegisterStyle(typeof(Log4NetViewModel), newStyle);

			return selectPanesStyle;
		}
		#endregion methods
	}
}
