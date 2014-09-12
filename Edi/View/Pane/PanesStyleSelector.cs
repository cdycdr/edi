namespace Edi.View.Pane
{
  using System.Windows;
  using System.Windows.Controls;
  using Edi.Core.ViewModels;
  using EdiViews.ViewModels.Document;
  using EdiViews.ViewModels.Document.Log4Net;
  using EdiViews.ViewModels.Document.MiniUml;
  using EdiViews.ViewModels.Document.StartPage;

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

    public override System.Windows.Style SelectStyle(object item, System.Windows.DependencyObject container)
    {
      // Lets use the file style for text and UML modeling since both editors
      // have similar capabilities (Load, Save, Edit, Undo, and so forth)
      if (item is EdiViewModel || item is MiniUmlViewModel)
        return this.FileStyle;

      if (item is StartPageViewModel)
        return this.StartPageStyle;

      if (item is Log4NetViewModel)
        return this.Log4NetStyle;

      if (item is ToolViewModel)
        return this.ToolStyle;

      return base.SelectStyle(item, container);
    }
  }
}
