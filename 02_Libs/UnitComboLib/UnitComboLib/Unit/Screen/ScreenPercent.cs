namespace UnitComboLib.Unit.Screen
{
  using System;

  public class ScreenPercent
  {
    private double mValue = 0;

    #region constructor
    public ScreenPercent(double value)
    {
      this.mValue = value;
    }

    private ScreenPercent()
    {
    }
    #endregion constructor

    #region methods
    public static double ToUnit(double inputValue, Itemkey targetUnit)
    {
      ScreenPercent d = new ScreenPercent(inputValue);

      return d.ToUnit(targetUnit);
    }

    public double ToUnit(Itemkey targetUnit)
    {
      switch (targetUnit)
      {
        case Itemkey.ScreenPercent:
          return this.mValue;

        case Itemkey.ScreenFontPoints:
          return (this.mValue * ScreenConverter.OneHundretPercentFont) / 100.0;

        default:
          throw new NotImplementedException(targetUnit.ToString());
      }
    }
    #endregion methods
  }
}
