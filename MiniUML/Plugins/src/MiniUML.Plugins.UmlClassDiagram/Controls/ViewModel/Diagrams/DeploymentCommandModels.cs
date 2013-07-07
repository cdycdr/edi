namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Diagrams
{
  using MiniUML.Framework.Command;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shapes;
  using System;
  using System.Collections.Generic;

  internal class DeploymentCommandModels
  {
    #region fields
    private ShapeDataDef[] mInitBaseData =
    {
      new ShapeDataDef(
      "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Deployment/ComponentToolBox.png",
      UmlKeys.UmlShapeSquare_Deployment,
      ControlKey.SquareShape,
      "Component",
      null,
      "Component",
      "Creates a component shape",
      "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Deployment/Component.png",
      false,
      125, 75
      ),
      new ShapeDataDef(
      "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Deployment/ComponentToolBox.png",
      UmlKeys.UmlShapeSquare_Node,
      ControlKey.NodeShape,
      "Node",
      null,
      "Node",
      "Creates a node shape",
      null,
      false,
      150, 75
      ),
      new ShapeDataDef(
      "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Deployment/ComponentToolBox.png",
      UmlKeys.UmlShapeSquare_Node,
      ControlKey.NodeShape,
      "Device",
      string.Format("{0}device{1}", (char)ShapeDataDef.StereotypeLeadIn, (char)ShapeDataDef.StereotypeLeadOut),
      "Device",
      "Creates a device shape",
      null,
      false,
      150, 75
      ),
      new ShapeDataDef(
      "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Deployment/ComponentToolBox.png",
      UmlKeys.UmlShapeSquare_Node,
      ControlKey.NodeShape,
      "ExecutionEnvironment",
      string.Format("{0}execution environment{1}", (char)ShapeDataDef.StereotypeLeadIn, (char)ShapeDataDef.StereotypeLeadOut),
      "ExecutionEnvironment",
      "Creates a execution environment shape",
      null,
      false,
      200, 75
      ),

      new ShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Interface.png",
        UmlKeys.UmlShapeSquare_Interface,
        ControlKey.SquareShape,
        "Interface1",
        string.Format("{0}interface{1}", (char)ShapeDataDef.StereotypeLeadIn, (char)ShapeDataDef.StereotypeLeadOut),
        "Interface",
        "Creates an interface shape",
        null, true
      ),

      new ShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Deployment/ComponentToolBox.png",
        UmlKeys.UmlShapeSquare_DeploymentSpec,
        ControlKey.SquareShape,
        "Deployment Specification",
        string.Format("{0}deployment spec{1}", (char)ShapeDataDef.StereotypeLeadIn, (char)ShapeDataDef.StereotypeLeadOut),
        "Deployment Specification",
        "Creates a deployment specification shape",
        null, false,
        175, 125
      )
    };
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard Constructor
    /// </summary>
    public DeploymentCommandModels()
    {
    }
    #endregion constructor

    #region Methods
    public IEnumerable<CommandModelBase> GetClassLikeShapes(PluginViewModel viewModel)
    {
      List<CommandModelBase> ret = new List<CommandModelBase>();

      foreach (ShapeDataDef item in this.mInitBaseData)
      {
        switch (item.ImplementingControl)
        {
          case ControlKey.SquareShape:
            ret.Add(new CreateSquareShapeCommandModel(viewModel,
                                                      item.ToolboxImageUrl,
                                                      item.ShapeKey,
                                                      item.ShapeName,
                                                      item.ShapeStereotype,
                                                      item.ToolboxName,
                                                      item.Description,
                                                      item.ShapeImageUrl,
                                                      item.ShapeHorizontalLine,
                                                      item.DefaultWidth, item.DefaultWidth));
            break;

          case ControlKey.DecisionShape:
            ret.Add(new CreateDecisionShapeCommandModel(viewModel,
                                                      item.ToolboxImageUrl,
                                                      item.ShapeKey,
                                                      item.ToolboxName,
                                                      item.Description));
            break;

          case ControlKey.NodeShape:
            ret.Add(new CreateNodeShapeCommandModel(viewModel,
                                                      item.ToolboxImageUrl,
                                                      item.ShapeKey,
                                                      item.ShapeName,
                                                      item.ShapeStereotype,
                                                      item.ToolboxName,
                                                      item.Description,
                                                      item.ShapeImageUrl,
                                                      item.DefaultWidth, item.DefaultWidth));
          break;

          default:
            break;
        }
      }

      return ret;
    }
    #endregion Methods

    #region private class
    private class ShapeDataDef
    {
      // Character code for '«' much smaller symbol typically used as lead-in on display of stereotype name
      public const int StereotypeLeadIn = 171;

      // Character code for '»' much greater symbol typically used as lead-out on display of stereotype name
      public const int StereotypeLeadOut = 187;

      public ShapeDataDef(string toolboxImageUrl,
                              string shapeKey,
                              ControlKey implementingControl,
                              string shapeName,
                              string shapeStereotype,
                              string toolboxName,
                              string description,
                              string shapeImageUrl = "",
                              bool shapeHorizontalLine = false,
                              double defaultWidth = 95,
                              double defaultHeight = 75)
      {
        this.ToolboxImageUrl = toolboxImageUrl;
        this.ShapeKey = shapeKey;
        this.ImplementingControl = implementingControl;
        this.ShapeName = shapeName;
        this.ShapeStereotype = shapeStereotype;
        this.ToolboxName = toolboxName;
        this.Description = description;
        this.ShapeImageUrl = shapeImageUrl;
        this.ShapeHorizontalLine = shapeHorizontalLine;

        this.DefaultHeight = defaultHeight;
        this.DefaultWidth = defaultWidth;
      }

      public string ToolboxImageUrl { get; private set; }
      public string ShapeKey { get; private set; }
      public ControlKey ImplementingControl { get; private set; }
      public string ShapeName { get; private set; }
      public string ShapeStereotype { get; private set; }
      public string ToolboxName { get; private set; }
      public string Description { get; private set; }
      public string ShapeImageUrl { get; private set; }
      public bool ShapeHorizontalLine { get; private set; }

      public double DefaultWidth { get; private set; }
      public double DefaultHeight { get; private set; }
    }
    #endregion private class
  }
}
