namespace EdiViews.Behaviour
{
  using System.Windows;

  public static class WindowMaximized
  {
    // Using a DependencyProperty as the backing store for IsMaximized.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty IsNotMaximizedProperty =
        DependencyProperty.RegisterAttached("IsNotMaximized",
                                            typeof(bool?),
                                            typeof(WindowMaximized),
                                            new PropertyMetadata(null, IsNotMaximizedChanged));

    public static bool? GetIsNotMaximized(DependencyObject obj)
    {
      return (bool?)obj.GetValue(IsNotMaximizedProperty);
    }

    public static void SetIsNotMaximized(DependencyObject obj, bool? value)
    {
      obj.SetValue(IsNotMaximizedProperty, value);
    }

    private static void IsNotMaximizedChanged(DependencyObject d,
                                              DependencyPropertyChangedEventArgs e)
    {
      var window = d as Window;

      if (window != null)
        window.StateChanged -= window_StateChanged;

      if (e != null)
      {
        if (e.NewValue == null)
          return;
      }

      if (window != null)
        window.StateChanged += window_StateChanged;
    }

    static void window_StateChanged(object sender, System.EventArgs e)
    {
      Window w = sender as Window;

      if (w != null)
      {
        if (w.WindowState == WindowState.Maximized)
          WindowMaximized.SetIsNotMaximized(w, false);
        else
          WindowMaximized.SetIsNotMaximized(w, true);
      }
    }
  }
}
