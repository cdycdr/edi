namespace Edi.View.Pane
{
  using System.Windows;
  using System.Windows.Controls;
  using EdiViews.Documents.Log4Net;
  using EdiViews.ViewModel.Documents.StartPage;
  using EdiViews.Tools.FileExplorer;
  using EdiViews.Tools.FileStats;
  using EdiViews.Tools.Log4Net;
  using EdiViews.Tools.RecentFiles;
  using EdiViews.ViewModel.Documents;
  using Xceed.Wpf.AvalonDock.Layout;

  /// <summary>
  /// Select a corresponding <seealso cref="DataTemplate"/> to a given type of viewmodel.
  /// </summary>
  class PanesTemplateSelector : DataTemplateSelector
  {
    public PanesTemplateSelector()
    {
    }

    public DataTemplate FileViewTemplate
    {
      get;
      set;
    }

    public DataTemplate MiniUMLViewTemplate
    {
      get;
      set;
    }

    public DataTemplate StartPageViewTemplate
    {
      get;
      set;
    }

    public DataTemplate FileExplorerViewTemplate
    {
      get;
      set;
    }

    public DataTemplate Log4NetViewTemplate
    {
      get;
      set;
    }

    public DataTemplate Log4NetToolViewTemplate
    {
      get;
      set;
    }

    public DataTemplate Log4NetMessageToolViewTemplate
    {
      get;
      set;
    }

    public DataTemplate RecentFilesViewTemplate
    {
      get;
      set;
    }

    public DataTemplate FileStatsViewTemplate
    {
      get;
      set;
    }

    public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
    {
      var itemAsLayoutContent = item as LayoutContent;

////      if (itemAsLayoutContent != null)
////      {
////      }

      if (item is EdiViewModel)
        return this.FileViewTemplate;

      if (item is MiniUmlViewModel)
        return this.MiniUMLViewTemplate;

      if (item is StartPageViewModel)
        return this.StartPageViewTemplate;

      if (item is FileExplorerViewModel)
        return this.FileExplorerViewTemplate;

      if (item is Log4NetViewModel)
        return this.Log4NetViewTemplate;

      if (item is Log4NetToolViewModel)
        return this.Log4NetToolViewTemplate;

      if (item is Log4NetMessageToolViewModel)
        return this.Log4NetMessageToolViewTemplate;

      if (item is FileStatsViewModel)
        return this.FileStatsViewTemplate;

      if (item is RecentFilesViewModel)
        return this.RecentFilesViewTemplate;

      return base.SelectTemplate(item, container);
    }
  }
}
