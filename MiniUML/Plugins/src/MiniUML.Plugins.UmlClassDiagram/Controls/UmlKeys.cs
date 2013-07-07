namespace MiniUML.Plugins.UmlClassDiagram.Controls
{
  public static class UmlKeys
  {
    public const string UmlShapeSquare_Primitive = "UML.Shape.Square.Primitive";
    public const string UmlShapeSquare_DataType = "UML.Shape.Square.DataType";
    public const string UmlShapeSquare_Signal = "UML.Shape.Square.Signal";

    public const string UmlShapeSquare_Class = "UML.Shape.Square.Class";
    public const string UmlShapeSquare_Interface = "UML.Shape.Square.Interface";
    public const string UmlShapeSquare_Table = "UML.Shape.Square.Table";
    public const string UmlShapeSquare_Enumeration = "UML.Shape.Square.Enumeration";

    public const string UmlShapeSquare_Decision = "UML.Shape.Decision";

    public const string UmlShapeSquare_Deployment = "UML.Shape.Square.Deployment.Component";
    public const string UmlShapeSquare_Node = "UML.Shape.Square.Deployment.Node";
    public const string UmlShapeSquare_DeploymentSpec = "UML.Shape.Square.Deployment.DeploymentSpec";

    // TODO XXX: Note/Commant key not defined, yet
    public const string UmlShape_Note = "UML.Shape.Note";
    public const string UmlShape_Package = "UML.Shape.Package";
    public const string UmlShape_Boundary = "UML.Shape.Boundary";

    public const string UmlShape_UseCase = "UML.Shape.UseCase.UseCase";
    public const string UmlShape_CanvasShape = "UML.Shape.UseCase.CanvasShape";
  }

  public enum PathSelector
  {
    Undefined = 0,
    UmlMan = 1,
    ActivityInitial = 2,
    ActivityFinal = 3,
    ActivityFlowFinal = 4,
    ActivitySync = 5,
    Event1 = 6,
    Event2 = 7
  }
}
