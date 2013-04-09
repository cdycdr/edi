namespace UnitComboLib.Unit
{
  /// <summary>
  /// Enumeration keys for each unit
  /// </summary>
  public enum Itemkey
  {
    ScreenFontPoints = 5,    // Units of computer screen dimensions
    ScreenPercent = 6
  }

  public abstract class Converter
  {
    public abstract double Convert(Itemkey inputUnit, double inputValue, Itemkey outputUnit);
  }
}
