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
  /// Creates a command model that has the ability to create an UML Comment Shape.
  /// </summary>
  public class CreateCommentShapeCommandModel : CommandModelBase, IDragableCommandModel
  {
    #region fields
    private string mToolBoxName = string.Empty;
    private string mToolBoxDescription = string.Empty;

    private string mText = string.Empty;

    double mDefaultWidth = 75.0,
           mDefaultHeight = 65.0;

    private PluginViewModel mViewModel;
    private RelayCommand<object> mCreateShape = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard cosntructor
    /// </summary>
    /// <param name="viewModel"></param>
    public CreateCommentShapeCommandModel(PluginViewModel viewModel,
                                                   string toolBoxImageUrl,
                                                   string toolBoxName,
                                                   string toolBoxDescription,
                                                   string text,
                                                   double defaultWidth = 125.0,
                                                   double defaultHeight = 65.0)
    {
      this.mToolBoxImageUrl = toolBoxImageUrl;
      this.mViewModel = viewModel;

      this.mToolBoxName = toolBoxName;
      this.mToolBoxDescription = toolBoxDescription;
      this.mText = text;
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
    /// Get description string for this canvase item.
    /// </summary>
    public string ToolTip
    {
      get
      {
        return this.mToolBoxDescription;
      }
    }

    /// <summary>
    /// Get title string for this canvase item.
    /// </summary>
    public string Title
    {
      get
      {
        return this.mToolBoxName;
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
      XElement element = new XElement(UmlKeys.UmlShape_Note,
                         new XAttribute("Text", this.mText),
                         new XAttribute("Top", dropPoint.Y),
                         new XAttribute("Left", dropPoint.X),
                         new XAttribute("Width", this.mDefaultWidth),
                         new XAttribute("Height", this.mDefaultHeight));

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
