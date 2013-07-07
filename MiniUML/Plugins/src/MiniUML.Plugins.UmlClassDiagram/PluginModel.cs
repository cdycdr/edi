namespace MiniUML.Plugins.UmlClassDiagram
{
  using System.Windows;
  using MiniUML.Model.ViewModels;

  public class PluginModel : MiniUML.Framework.PluginModel
  {
    #region field
    private FrameworkElement mPluginView;
    #endregion field

    #region constructor
    public PluginModel(IMiniUMLDocument windowViewModel)
    {
      this.mPluginView = new PluginView();
      this.mPluginView.DataContext = new PluginViewModel(windowViewModel);
    }
    #endregion constructor

    #region properties

    public override string Name
    {
      get { return MiniUML.Framework.Local.Strings.STR_UML_CLASS_DIAGRAM_TT; }
    }

    public override FrameworkElement View
    {
      get { return this.mPluginView; }
    }
    #endregion properties
  }
}
