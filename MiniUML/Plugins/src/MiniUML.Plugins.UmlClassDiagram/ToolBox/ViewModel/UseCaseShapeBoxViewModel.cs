namespace MiniUML.Plugins.UmlClassDiagram.ToolBox.ViewModel
{
  using System.Collections.Generic;
  using MiniUML.Framework.Command;
  using MiniUML.Framework.helpers;
  using MiniUML.Plugins.UmlClassDiagram.Controls;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shapes;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Diagrams;

  /// <summary>
  /// A viewmodel class to manage the viewmodel for a toolbox of a canvas.
  /// 
  /// Source: http://www.codeproject.com/Articles/484616/MVVM-Diagram-Designer?msg=4413242#Drag-And-Drop-To-The-Design-Surface
  /// </summary>
  public class UseCaseShapeBoxViewModel : IShapBox
  {
    private List<ToolBoxData> toolBoxItems = new List<ToolBoxData>();

    /// <summary>
    /// Class constructor to
    /// create all shapes that have similar properties as a class shape
    /// </summary>
    /// <param name="pluginViewModel"></param>
    public UseCaseShapeBoxViewModel(PluginViewModel pluginViewModel)
    {
      var classShapes = new UseCaseCommandModels();

      foreach (var item in classShapes.GetClassLikeShapes(pluginViewModel))
      {
        this.toolBoxItems.Add(new ToolBoxData(item.ToolBoxImageUrl, item));
      }

      var commonShapes = new CommonCommandModels();

      foreach (var item in commonShapes.GetClassLikeShapes(pluginViewModel))
      {
        this.toolBoxItems.Add(new ToolBoxData(item.ToolBoxImageUrl, item));
      }
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