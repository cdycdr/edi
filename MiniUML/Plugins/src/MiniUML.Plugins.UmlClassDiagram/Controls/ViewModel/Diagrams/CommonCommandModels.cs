namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Diagrams
{
  using MiniUML.Framework.Command;
  using MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shapes;
  using System.Collections.Generic;

  internal class CommonCommandModels
  {
    #region fields
    private ClassShapeDataDef[] mInitBaseData =
    {
      new ClassShapeDataDef(
      "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Common/Package.png",
      UmlKeys.UmlShape_Package,
      ControlKey.PackageShape,
      "Package",
      null,
      "Package",
      "Creates a package shape",
      null,
      false,
      125, 100
      ),
      new ClassShapeDataDef(
      "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Common/Boundary.png",
      UmlKeys.UmlShape_Boundary,
      ControlKey.BoundaryShape,
      "Boundary",
      null,
      "Boundary",
      "Creates a boundary shape",
      null,
      false,
      700, 500
      ),
      new ClassShapeDataDef(
      "/MiniUML.Plugins.UmlClassDiagram;component/Images/Shapes/Common/Note.png",
      UmlKeys.UmlShape_Note,
      ControlKey.NoteShape,
      "Note",
      null,
      "Note",
      "Creates a note shape"
      )
    };
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard Constructor
    /// </summary>
    public CommonCommandModels()
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
          case ControlKey.PackageShape:
            ret.Add(new CreatePackageShapeCommandModel(viewModel,
                                                      item.ToolboxImageUrl,
                                                      item.ShapeKey,
                                                      item.ShapeName,
                                                      item.ToolboxName,
                                                      item.ToolBoxDescription,
                                                      item.DefaultWidth,
                                                      item.DefaultHeight));
            break;

          case ControlKey.BoundaryShape:
            ret.Add(new CreateBoundaryShapeCommandModel(viewModel,
                                                        item.ToolboxImageUrl,
                                                        item.ShapeKey,
                                                        item.ShapeName,
                                                        item.ToolboxName,
                                                        item.ToolBoxDescription,
                                                        item.DefaultWidth,
                                                        item.DefaultHeight));
            break;

            case ControlKey.NoteShape:                  // Create a comment shape
              ret.Add(new CreateCommentShapeCommandModel(viewModel,
                                                         item.ToolboxImageUrl,
                                                         item.ToolboxName,
                                                         item.ToolBoxDescription,
                                                         item.ToolboxName            // Text property
                                                         ));
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
        this.ToolBoxDescription = description;
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
      public string ToolBoxDescription { get; private set; }
      public string ShapeImageUrl { get; private set; }
      public bool ShapeHorizontalLine { get; private set; }

      public double DefaultWidth { get; private set; }
      public double DefaultHeight { get; private set; }
    }
    #endregion private class
  }
}
