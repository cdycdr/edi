namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Connect
{
  using System.Windows;
  using System.Windows.Input;
  using System.Xml.Linq;
  using MiniUML.Framework;
  using MiniUML.Framework.Command;
  using MiniUML.Framework.interfaces;
  using MiniUML.Model.ViewModels;

  /// <summary>
  /// Creates a command model that has the ability to create an UML Association Connector Shape.
  /// </summary>
  public class CreateAggregationShapeCommandModel : CommandModelBase, IDragableCommandModel
  {
    #region fields
    public static readonly string Name = MiniUML.Framework.Local.Strings.STR_CMD_Aggregation;
    public static readonly string Description = MiniUML.Framework.Local.Strings.STR_CMD_Aggregation_description;

    private PluginViewModel mViewModel;
    private RelayCommand<object> mCreateShape = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard cosntructor
    /// </summary>
    /// <param name="viewModel"></param>
    public CreateAggregationShapeCommandModel(PluginViewModel viewModel)
    {
      this.mToolBoxImageUrl = "/MiniUML.Plugins.UmlClassDiagram;component/Images/Connect/Command.CreateAggregationShape.png";
      this.mViewModel = viewModel;
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get property to command which can create a new shape.
    /// </summary>
    public override ICommand CreateShape
    {
      get
      {
        if (this.mCreateShape == null)
          this.mCreateShape = new RelayCommand<object>((p) => this.OnExecute(), (p) => this.OnQueryEnabled());

        return this.mCreateShape;
      }
    }

    /// <summary>
    /// Get description string for this canvase item.
    /// </summary>
    public string ToolTip
    {
      get
      {
        return CreateAggregationShapeCommandModel.Description;
      }
    }

    /// <summary>
    /// Get title string for this canvase item.
    /// </summary>
    public string Title
    {
      get
      {
        return CreateAggregationShapeCommandModel.Name;
      }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Method is required by <seealso cref="IDragableCommandModel"/>. It is executed
    /// when the drag & drop operation on the canvas is infished with its last step
    /// (the creation of the viewmodel for the new item).
    /// </summary>
    /// <param name="dropPoint"></param>
    public void OnDragDropExecute(Point dropPoint)
    {
      var model = this.mViewModel.mWindowViewModel.vm_DocumentViewModel;

      model.v_CanvasView.ForceCursor = true;
      model.v_CanvasView.Cursor = Cursors.Pen;

      model.vm_CanvasViewModel.BeginCanvasViewMouseHandler(
          new CreateAssociationMouseHandler(model, new XElement("Uml.Association.Aggregation")));
    }

    #region CreateShape Command
    private bool OnQueryEnabled()
    {
      if (this.mViewModel == null)
        return false;

      return this.mViewModel.mWindowViewModel.vm_DocumentViewModel.dm_DocumentDataModel.State == DataModel.ModelState.Ready;
    }

    private void OnExecute()
    {
      this.OnDragDropExecute(new Point(100, 10));
    }
    #endregion CreateShape Command
    #endregion methods

    #region private class
    private class CreateAssociationMouseHandler : AbstractCreateAssociationMouseHandler
    {
      ////private XElement mFromElement;

      public CreateAssociationMouseHandler(AbstractDocumentViewModel viewModel, XElement association)
        : base(viewModel, association)
      {
      }

      protected override bool IsValidFrom(XElement element)
      {
        return true;

        /***
        string[] validShapes = { "Uml.Interface", "Uml.Class", "Uml.AbstractClass", "Uml.Struct" };

                  _fromElement = element;

                  foreach (string s in validShapes)
                    if (element.Name == s) return true;

                  return false;
         ***/
      }

      protected override bool IsValidTo(XElement element)
      {
        return true;
        /***
        string[] validShapes = { "Uml.Interface", "Uml.Class", "Uml.AbstractClass", "Uml.Struct", "Uml.Enum" };

        foreach (string s in validShapes)
          if (element.Name == s) return true;

        return false;
        ***/
      }
    }
    #endregion private classe
  }
}
