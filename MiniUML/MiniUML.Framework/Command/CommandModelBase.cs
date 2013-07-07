namespace MiniUML.Framework.Command
{
  using System.Windows.Input;

  /// <summary>
  /// Model for a command
  /// </summary>
  public abstract class CommandModelBase
  {
    #region fields
    protected string mToolBoxImageUrl = string.Empty;
    #endregion fields

    #region Constructors
    /// <summary>
    /// Standard constructor
    /// </summary>
    public CommandModelBase()
    {
    }
    #endregion

    #region Properties
    /// <summary>
    /// Get Url string of image representation of this command in GUI
    /// </summary>
    public virtual string ToolBoxImageUrl
    {
     get
     {
       return this.mToolBoxImageUrl;
     }
   }

    /// <summary>
    /// Command to be executed via this model
    /// </summary>
    abstract public ICommand CreateShape { get; }
    #endregion Properties
  }
}
