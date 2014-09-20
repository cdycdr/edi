namespace EdiApp.Converters
{
  using System.Windows;

  /// <summary>
  /// WPF Converter class to convert boolean values into visibility values.
  /// </summary>
  public sealed class BooleanNotConverter : BooleanConverter<bool>
  {
    /// <summary>
    /// Class Constructor
    /// </summary>
    public BooleanNotConverter() :
      base(false, true)
      {
      }
  }
}
