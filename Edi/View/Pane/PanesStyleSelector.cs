namespace Edi.View.Pane
{
  using System.Windows;
  using System.Windows.Controls;
  using Edi.ViewModel;
  using EdiViews.ViewModel.Base;

  /// <summary>
  /// Select a corresponding style for a given viewmodel.
  /// </summary>
  internal class PanesStyleSelector : StyleSelector
  {
    public Style ToolStyle
    {
      get; set;
    }

    public Style FileStyle
    {
      get; set;
    }

    public Style StartPageStyle
    {
      get; set;
    }

    public Style Log4NetStyle
    {
      get;
      set;
    }

    public Style Log4NetToolViewStyle
    {
      get;
      set;
    }

    public Style Log4NetMessageToolViewStyle
    {
      get;
      set;
    }

    public Style RecentFilesStyle
    {
      get; set;
    }

    public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
    {
      if (item is ToolViewModel)
        return this.ToolStyle;

      if (item is EdiViewModel)
        return this.FileStyle;

      if (item is StartPageViewModel)
        return this.StartPageStyle;

      if (item is Log4NetViewModel)
        return this.Log4NetStyle;

      if (item is Log4NetToolViewModel)
        return this.Log4NetToolViewStyle;

      if (item is Log4NetMessageToolViewModel)
        return this.Log4NetMessageToolViewStyle;
        
      if (item is RecentFilesViewModel)
        return this.FileStyle;

      return base.SelectStyle(item, container);
    }
  }
}
