namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Diagrams
{
  using MiniUML.Framework.Command;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shapes;
  using System.Collections.Generic;

  internal class ActivityCommandModels
  {
    #region fields
    private ClassShapeDataDef[] mInitBaseData =
    {
      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/DataType.png",
        UmlKeys.UmlShape_CanvasShape,
        ControlKey.CanvasShape,
        "Activity Final",
        null,
        "Activity Final",
        "Creates an final activity shape",
        60, 105,
        null,
        PathSelector.ActivityFinal
        ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/DataType.png",
        UmlKeys.UmlShape_CanvasShape,
        ControlKey.CanvasShape,
        "Activity Flow Final",
        null,
        "Activity Flow Final",
        "Creates an final activity flow shape",
        60, 105,
        null,
        PathSelector.ActivityFlowFinal
        ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/DataType.png",
        UmlKeys.UmlShape_CanvasShape,
        ControlKey.CanvasShape,
        "Activity Initial",
        null,
        "Activity Initial",
        "Creates an initial activity shape",
        60, 105,
        null,
        PathSelector.ActivityInitial
        ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/DataType.png",
        UmlKeys.UmlShape_CanvasShape,
        ControlKey.CanvasShape,
        "Activity Sync",
        null,
        "Activity Sync",
        "Creates an activity sync (man symbol)",
        60, 105,
        null,
        PathSelector.ActivitySync
        ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/DataType.png",
        UmlKeys.UmlShape_CanvasShape,
        ControlKey.CanvasShape,
        "Event",
        null,
        "Event",
        "Creates an event shape",
        105, 50,
        null,
        PathSelector.Event1
        ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/DataType.png",
        UmlKeys.UmlShape_CanvasShape,
        ControlKey.CanvasShape,
        "Event",
        null,
        "Event",
        "Creates an event shape",
        105, 50,
        null,
        PathSelector.Event2
        ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/DataType.png",
        UmlKeys.UmlShape_CanvasShape,
        ControlKey.CanvasShape,
        "Action",
        null,
        "Action",
        "Creates an action shape",
        105, 50,
        null,
        PathSelector.Event1
        ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/DataType.png",
        UmlKeys.UmlShape_CanvasShape,
        ControlKey.CanvasShape,
        "Action",
        null,
        "Action",
        "Creates an action shape",
        105, 50,
        null,
        PathSelector.Event2
        )
    };
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ActivityCommandModels()
    {
    }
    #endregion constructor

    #region Methods
    public IEnumerable<CommandModelBase> GetClassLikeShapes(PluginViewModel viewModel)
    {
      List<CommandModelBase> ret = new List<CommandModelBase>();

      foreach (ClassShapeDataDef item in this.mInitBaseData)
      {
        switch (item.ImplementingControl)
        {
          case ControlKey.UseCaseShape:
            ret.Add(new CreateUseCaseShapeCommandModel(viewModel,
                                                      item.ToolboxImageUrl,
                                                      item.ShapeKey,
                                                      item.ShapeName,
                                                      item.ToolboxName,
                                                      item.Description,
                                                      item.DefaultWidth, item.DefaultHeight,
                                                      item.StrokeDashArray));
            break;

          case ControlKey.SquareShape:
            ret.Add(new CreateSquareShapeCommandModel(viewModel,
                                                      item.ToolboxImageUrl,
                                                      item.ShapeKey,
                                                      item.ShapeName,
                                                      item.ShapeStereotype,
                                                      item.ToolboxName,
                                                      item.Description,
                                                      null,
                                                      false,
                                                      item.DefaultWidth, item.DefaultHeight));
            break;

          case ControlKey.CanvasShape:
            ret.Add(new CreateCanvaseShapeCommandModel(viewModel,
                                                      item.ToolboxImageUrl,
                                                      item.ShapeKey,
                                                      item.ShapeName,
                                                      item.ToolboxName,
                                                      item.Description,
                                                      item.DefaultWidth, item.DefaultHeight,
                                                      item.CanvasToDisplay));
            break;

          default:
            break;
        }
      }

      return ret;
    }
    #endregion Methods

    #region private class
    private class ClassShapeDataDef
    {
      // Character code for '«' much smaller symbol typically used as lead-in on display of stereotype name
      public const int StereotypeLeadIn = 171;

      // Character code for '»' much greater symbol typically used as lead-out on display of stereotype name
      public const int StereotypeLeadOut = 187;

      public ClassShapeDataDef()
      {
        this.StrokeDashArray = null;
        this.CanvasToDisplay = PathSelector.Undefined;
      }

      /// <summary>
      /// Class Constructor
      /// </summary>
      /// <param name="toolboxImageUrl"></param>
      /// <param name="shapeKey"></param>
      /// <param name="implementingControl"></param>
      /// <param name="shapeName"></param>
      /// <param name="shapeStereotype"></param>
      /// <param name="toolboxName"></param>
      /// <param name="description"></param>
      /// <param name="shapeImageUrl"></param>
      /// <param name="shapeHorizontalLine"></param>
      /// <param name="defaultWidth"></param>
      /// <param name="defaultHeight"></param>
      public ClassShapeDataDef(string toolboxImageUrl,
                              string shapeKey,
                              ControlKey implementingControl,
                              string shapeName,
                              string shapeStereotype,
                              string toolboxName,
                              string description,
                              double defaultWidth = 95,
                              double defaultHeight = 75,
                              string strokeDashArray = null,
                              PathSelector canvasToDisplay = PathSelector.Undefined)
        : this()
      {
        this.ToolboxImageUrl = toolboxImageUrl;
        this.ShapeKey = shapeKey;
        this.ImplementingControl = implementingControl;
        this.ShapeName = shapeName;
        this.ShapeStereotype = shapeStereotype;
        this.ToolboxName = toolboxName;
        this.Description = description;
        ////this.ShapeImageUrl = shapeImageUrl;
        ////this.ShapeHorizontalLine = shapeHorizontalLine;

        this.DefaultHeight = defaultHeight;
        this.DefaultWidth = defaultWidth;

        this.StrokeDashArray = strokeDashArray;

        this.CanvasToDisplay = canvasToDisplay;
      }

      public string ToolboxImageUrl { get; private set; }
      public string ShapeKey { get; private set; }
      public ControlKey ImplementingControl { get; private set; }
      public string ShapeName { get; private set; }
      public string ShapeStereotype { get; private set; }
      public string ToolboxName { get; private set; }
      public string Description { get; private set; }
      ////public string ShapeImageUrl { get; private set; }
      ////public bool ShapeHorizontalLine { get; private set; }

      public double DefaultWidth { get; private set; }
      public double DefaultHeight { get; private set; }

      public string StrokeDashArray { get; private set; }

      public PathSelector CanvasToDisplay { get; private set; } 
    }
    #endregion private class
  }
}
