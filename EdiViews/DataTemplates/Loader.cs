namespace EdiViews.DataTemplates
{
  using System.Reflection;
  using System.Windows;
  using Edi.Core.Resources;
  using Edi.Core.View.Pane;
  using EdiViews.Tools.FileExplorer;
  using EdiViews.Tools.FileStats;
  using EdiViews.Tools.Log4Net;
  using EdiViews.Tools.RecentFiles;
  using EdiViews.ViewModels.Document;
  using EdiViews.ViewModels.Document.Log4Net;
  using EdiViews.ViewModels.Document.MiniUml;
  using EdiViews.ViewModels.Document.StartPage;

  public class Loader
  {
    /// <summary>
    /// Register viewmodel types with <seealso cref="DataTemplate"/> for a view
    /// and return all definitions with a <seealso cref="PanesTemplateSelector"/> instance.
    /// </summary>
    /// <param name="paneSel"></param>
    /// <returns></returns>
    public static PanesTemplateSelector RegisterDataTemplates(PanesTemplateSelector paneSel)
    {
      // FileStatsView
      var template = ResourceLocator.GetResource<DataTemplate>(
                      Assembly.GetAssembly(typeof(FileStatsViewModel)).GetName().Name,
                      "DataTemplates/FileStatsViewDataTemplate.xaml",
                      "FileStatsViewDataTemplate") as DataTemplate;

      paneSel.RegisterDataTemplate(typeof(FileStatsViewModel), template);

      // StartPageView
      template = ResourceLocator.GetResource<DataTemplate>(
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

      // FileExplorer
      template = ResourceLocator.GetResource<DataTemplate>(
                  Assembly.GetAssembly(typeof(FileExplorerViewModel)).GetName().Name,
                  "DataTemplates/FileExplorerViewDataTemplate.xaml",
                  "FileExplorerViewDataTemplate") as DataTemplate;

      paneSel.RegisterDataTemplate(typeof(FileExplorerViewModel), template);

      return paneSel;
    }
  }
}
