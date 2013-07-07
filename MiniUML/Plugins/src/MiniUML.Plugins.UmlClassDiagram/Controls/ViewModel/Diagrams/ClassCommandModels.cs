namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Diagrams
{
  using MiniUML.Framework.Command;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shapes;
  using System.Collections.Generic;

  internal class ClassCommandModels
  {
    #region fields
    private ClassShapeDataDef[] mInitBaseData =
    {
      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Primitive.png",
        UmlKeys.UmlShapeSquare_Primitive,
        ControlKey.SquareShape,
        "PrimitiveType1",
        string.Format("{0}primitive{1}", (char)ClassShapeDataDef.StereotypeLeadIn, (char)ClassShapeDataDef.StereotypeLeadOut),
        "Primitive",
        "Creates a primitive shape",
        null,
        false,
        125.0, 75.0),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/DataType.png",
        UmlKeys.UmlShapeSquare_DataType,
        ControlKey.SquareShape,
        "DataType1",
        string.Format("{0}datatype{1}", (char)ClassShapeDataDef.StereotypeLeadIn, (char)ClassShapeDataDef.StereotypeLeadOut),
        "Data Type",
        "Creates a data type shape",
        null
        ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Signal.png",
        UmlKeys.UmlShapeSquare_DataType,
        ControlKey.SquareShape,
        "Signal1",
        string.Format("{0}signal{1}", (char)ClassShapeDataDef.StereotypeLeadIn, (char)ClassShapeDataDef.StereotypeLeadOut),
        "Signal",
        "Creates a signal shape",
        null
      ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Class.png",
        UmlKeys.UmlShapeSquare_Class,
        ControlKey.SquareShape,
        "Class1",
        null,
        "Class",
        "Creates a class shape",
        null, true
      ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/TableToolBox.png",
        UmlKeys.UmlShapeSquare_Table,
        ControlKey.SquareShape,
        "Table1",
        null,
        "Table",
        "Creates a table shape",
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Table.png", true
      ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Enumeration.png",
        UmlKeys.UmlShapeSquare_Enumeration,
        ControlKey.SquareShape,
        "Enumeration1",
        string.Format("{0}enumeration{1}", (char)ClassShapeDataDef.StereotypeLeadIn, (char)ClassShapeDataDef.StereotypeLeadOut),
        "Enumeration",
        "Creates an enumeration shape",
        null, true,
        125, 75
      ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Interface.png",
        UmlKeys.UmlShapeSquare_Interface,
        ControlKey.SquareShape,
        "Interface1",
        string.Format("{0}interface{1}", (char)ClassShapeDataDef.StereotypeLeadIn, (char)ClassShapeDataDef.StereotypeLeadOut),
        "Interface",
        "Creates an interface shape",
        null, true
      ),

      new ClassShapeDataDef(
        "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Class/Association.png",
        UmlKeys.UmlShapeSquare_Decision,
        ControlKey.DecisionShape,
        null,
        null,
        "Association",
        "Creates an association shape"
      ),
    };
    #endregion fields

    #region constructor
    public ClassCommandModels()
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
                                                      item.DefaultWidth, item.DefaultHeight));
            break;

          case ControlKey.DecisionShape:
            ret.Add(new CreateDecisionShapeCommandModel(viewModel,
                                                      item.ToolboxImageUrl,
                                                      item.ShapeKey,
                                                      item.ToolboxName,
                                                      item.Description));
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
