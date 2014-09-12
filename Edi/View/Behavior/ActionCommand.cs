namespace Edi.View.Behavior
{
  using System;
  using System.Diagnostics.CodeAnalysis;
  using System.Windows.Input;

  /// <summary>
  /// Execute an action via <seealso cref="ICommand"/> interface.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class ActionCommand<T> : ICommand
  {
    private Action<T> _action;

    /// <summary>
    /// Class Constructor
    /// </summary>
    /// <param name="action"></param>
    public ActionCommand(Action<T> action)
    {
      this._action = action;
      this.CanExecuteChanged = null;
    }

    // Disable event never used warning.
    #pragma warning disable 67
    /// <summary>
    /// Event to determine whether this document can execute or not.
    /// </summary>
    public event EventHandler CanExecuteChanged;

    // Enable event never used warning.
    #pragma warning restore 67

    public bool CanExecute(object parameter)
    {
      return true;
    }

    public void Execute(object parameter)
    {
      if (this._action != null)
      {
        var castParameter = (T)Convert.ChangeType(parameter, typeof(T));
        this._action(castParameter);
      }
    }
  }
}
