namespace MiniUML.Plugins.UmlClassDiagram.ToolBox.ViewModel
{
  using System.Collections.Generic;
  using MiniUML.Framework.Command;
  using MiniUML.Framework.helpers;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Connect;

  /// <summary>
  /// A viewmodel class to manage the viewmodel for a toolbox of a canvas.
  /// 
  /// Source: http://www.codeproject.com/Articles/484616/MVVM-Diagram-Designer?msg=4413242#Drag-And-Drop-To-The-Design-Surface
  /// </summary>
  public class ConnectBoxViewModel
  {
    private List<ToolBoxData> toolBoxItems = new List<ToolBoxData>();

    /// <summary>
    /// Parameterized class constructor
    /// </summary>
    /// <param name="pluginViewModel"></param>
    public ConnectBoxViewModel(PluginViewModel pluginViewModel)
    {
      CommandModelBase b = new CreateAggregationShapeCommandModel(pluginViewModel);
      this.toolBoxItems.Add(new ToolBoxData(b.ToolBoxImageUrl, b));

      b = new CreateAssociationShapeCommandModel(pluginViewModel);
      this.toolBoxItems.Add(new ToolBoxData(b.ToolBoxImageUrl, b));

      b = new CreateCompositionShapeCommandModel(pluginViewModel);
      this.toolBoxItems.Add(new ToolBoxData(b.ToolBoxImageUrl, b));

      b = new CreateInheritanceShapeCommandModel(pluginViewModel);
      this.toolBoxItems.Add(new ToolBoxData(b.ToolBoxImageUrl, b));
    }

    /// <summary>
    /// Get tool box items managed in this viewmodel
    /// </summary>
    public List<ToolBoxData> ToolBoxItems
    {
      get { return this.toolBoxItems; }
    }
  }
}