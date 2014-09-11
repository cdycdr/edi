namespace Edi.Core.ViewModels
{
  /// <summary>
  /// AvalonDock base class viewm-model to support tool window views
  /// </summary>
  public class ToolViewModel : PaneViewModel
  {
    /// <summary>
    /// Base constructor from nam of tool window item
    /// </summary>
    /// <param name="name">Name of tool window displayed in GUI</param>
    public ToolViewModel(string name)
    {
      Name = name;
      Title = name;
    }

    public string Name
    {
      get;
      private set;
    }

    #region IsVisible

    private bool _isVisible = true;
    public bool IsVisible
    {
      get { return _isVisible; }
      set
      {
        if (_isVisible != value)
        {
          _isVisible = value;
          RaisePropertyChanged("IsVisible");
        }
      }
    }

    #endregion
  }
}
