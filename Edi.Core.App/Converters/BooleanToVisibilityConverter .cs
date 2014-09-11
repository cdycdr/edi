namespace Edi.Core.App.Converters
{
  using System.Windows;

  /// <summary>
  /// WPF Converter class to convert boolean values into visibility values.
  /// </summary>
  public sealed class BooleanToVisibilityConverter : BooleanConverter<Visibility>
  {
    /// <summary>
    /// Class Constructor
    /// </summary>
    public BooleanToVisibilityConverter() :
      base(Visibility.Visible, Visibility.Collapsed)
    {
    }
  }
}
