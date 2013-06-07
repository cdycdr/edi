namespace MiniUML.Plugins.UmlClassDiagram
{
  using MiniUML.Model.ViewModels;
  using System.Windows;

  public class PluginModel : MiniUML.Framework.PluginModel
  {
    #region field
    private FrameworkElement _pluginView;
    #endregion field

    #region constructor
    public PluginModel(IMiniUMLDocument windowViewModel)
    {
      _pluginView = new PluginView();
      _pluginView.DataContext = new PluginViewModel(windowViewModel);
    }
    #endregion constructor

    #region properties
    public override string Name
    {
      get { return MiniUML.Framework.Local.Strings.STR_UML_CLASS_DIAGRAM_TT; }
    }

    public override FrameworkElement View
    {
      get { return _pluginView; }
    }
    #endregion properties
  }
}
