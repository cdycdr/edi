namespace FileListView.Views
{
  using System.Windows;

  /// <summary>
  /// Source: http://www.thomaslevesque.com/2011/03/21/wpf-how-to-bind-to-data-when-the-datacontext-is-not-inherited/
  ///  Issue: http://stackoverflow.com/questions/9994241/mvvm-binding-command-to-contextmenu-item
  /// </summary>
  public class BindingProxy : Freezable
  {
    public static readonly DependencyProperty DataProperty =
        DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new UIPropertyMetadata(null));

    /// <summary>
    /// Overrides of Freezable
    /// </summary>
    /// <returns></returns>
    protected override Freezable CreateInstanceCore()
    {
      return new BindingProxy();
    }

    public object Data
    {
      get { return (object)GetValue(DataProperty); }
      set { SetValue(DataProperty, value); }
    }
  }
}
