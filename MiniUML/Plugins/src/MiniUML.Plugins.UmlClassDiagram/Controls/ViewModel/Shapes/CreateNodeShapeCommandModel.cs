namespace MiniUML.Plugins.UmlClassDiagram.Controls.ViewModel.Shapes
{
  using System.Windows;
  using System.Windows.Input;
  using System.Xml.Linq;
  using MiniUML.Framework;
  using MiniUML.Framework.Command;
  using MiniUML.Framework.interfaces;
  using MiniUML.Model.ViewModels;

  /// <summary>
  /// Creates a command model that has the ability to create an UML Class Shape.
  /// </summary>
  internal class CreateNodeShapeCommandModel : CommandModelBase, IDragableCommandModel
  {
    #region fields
    private readonly string mShapeKey        = string.Empty;
    private readonly string mShapeName       = string.Empty;
    private readonly string mShapeStereotype = string.Empty;
    private readonly string mShapeImageUrl   = string.Empty;

    private readonly string mDisplayName = string.Empty;
    private readonly string mDescription     = string.Empty;

    double mDefaultWidth = 75.0,
           mDefaultHeight = 95.0;

    private PluginViewModel mViewModel;
    private RelayCommand<object> mCreateShape = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="viewModel"></param>
    public CreateNodeShapeCommandModel(PluginViewModel viewModel,
                                         string toolboxImageUrl,
                                         string shapeKey,
                                         string shapeName,
                                         string shapeStereotype,
                                         string toolboxName,    
                                         string description,
                                         string shapeImageUrl = "",
                                         double defaultWidth = 75.0,
                                         double defaultHeight = 95.0)
    {
      this.mViewModel = viewModel;
      this.mToolBoxImageUrl = toolboxImageUrl;
      this.mDisplayName = toolboxName;
      this.mDescription = description;

      this.mShapeKey = shapeKey;
      this.mShapeName = shapeName;
      this.mShapeStereotype = shapeStereotype;
      this.mShapeImageUrl = shapeImageUrl;

      this.mDefaultWidth = defaultWidth;
      this.mDefaultHeight = defaultHeight;
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
    /// Get description string for this canvase item  (for display in toolbox).
    /// </summary>
    public string ToolTip
    {
      get
      {
        return this.mDescription;
      }
    }

    /// <summary>
    /// Get title string for this canvase item (for display in toolbox).
    /// </summary>
    public string Title
    {
      get
      {
        return this.mDisplayName;
      }
    }
    #endregion constructor

    #region methods
    /// <summary>
    /// Method is required by <seealso cref="IDragableCommandModel"/>. It is executed
    /// when the drag & drop operation on the canvas is infished with its last step
    /// (the creation of the viewmodel for the new item).
    /// </summary>
    /// <param name="dropPoint"></param>
    public void OnDragDropExecute(Point dropPoint)
    {
      XElement element = new XElement(                         this.mShapeKey,
                         new XAttribute("Stereotype"         , this.mShapeStereotype == null ? string.Empty : this.mShapeStereotype),
                         new XAttribute("Name"               , this.mShapeName),
                         new XAttribute("ShapeImageUrl"      , this.mShapeImageUrl == null ? string.Empty : mShapeImageUrl),
                         new XAttribute("Top"                , dropPoint.Y),
                         new XAttribute("Left"               , dropPoint.X),
                         new XAttribute("Width"              , this.mDefaultWidth),
                         new XAttribute("Height"             , this.mDefaultHeight));

      AbstractDocumentViewModel documentViewModel = this.mViewModel.mWindowViewModel.vm_DocumentViewModel;
      documentViewModel.dm_DocumentDataModel.DocumentRoot.Add(element);
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
  }
}
