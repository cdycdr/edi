namespace MiniUML.Plugins.UmlClassDiagram
{
  using MiniUML.Framework;
  using MiniUML.Model.ViewModels;
  using MiniUML.Plugins.UmlClassDiagram.ToolBox.ViewModel;

  public partial class PluginViewModel : ViewModel
  {
    #region fields
    private ClassShapeBoxViewModel mClassShapeBox = null;
    private DeploymentShapeBoxViewModel mDeploymentShapeBox = null;
    private UseCaseShapeBoxViewModel mUseCaseShapeBoxViewModel = null;
    private ActivityShapeBoxViewModel mActivityShapeBoxViewModel = null;
    #endregion fields

    #region constructor
    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="windowViewModel"></param>
    public PluginViewModel(IMiniUMLDocument windowViewModel)
    {
      // Store a reference to the parent view model.
      this.mWindowViewModel = windowViewModel;

      this.ConnectBox = new ConnectBoxViewModel(this);
    }
    #endregion constructor

    #region properties
    /// <summary>
    /// Get a collection of commands which can be used to create Class shapes on the canvas.
    /// </summary>
    public ClassShapeBoxViewModel ClassShapeBox
    {
      get
      {
        if (this.mClassShapeBox == null)
          this.mClassShapeBox = new ClassShapeBoxViewModel(this);

        return this.mClassShapeBox;
      }
    }

    /// <summary>
    /// Get a collection of commands which can be used to create Deployment shapes on the canvas.
    /// </summary>
    public DeploymentShapeBoxViewModel DeploymentShapeBox
    {
      get
      {
        if (this.mDeploymentShapeBox == null)
          this.mDeploymentShapeBox = new DeploymentShapeBoxViewModel(this);

        return this.mDeploymentShapeBox;
      }
    }

    public UseCaseShapeBoxViewModel UseCaseShapeBox
    {
      get
      {
        if (this.mUseCaseShapeBoxViewModel == null)
          this.mUseCaseShapeBoxViewModel = new UseCaseShapeBoxViewModel(this);

        return this.mUseCaseShapeBoxViewModel;
      }
    }

    public ActivityShapeBoxViewModel ActivityShapeBox
    {
      get
      {
        if (this.mActivityShapeBoxViewModel == null)
          this.mActivityShapeBoxViewModel = new ActivityShapeBoxViewModel(this);

        return this.mActivityShapeBoxViewModel;
      }
    }

    /// <summary>
    /// Get a collection of commands which can be used to create connections between shapes on the canvas.
    /// </summary>
    public ConnectBoxViewModel ConnectBox { get; private set; }

    /// <summary>
    /// Get the current <seealso cref="IMiniUMLDocument"/> which contains the
    /// document with shapes and other items.
    /// </summary>
    public IMiniUMLDocument mWindowViewModel { get; private set; }
    #endregion properties
  }
}
