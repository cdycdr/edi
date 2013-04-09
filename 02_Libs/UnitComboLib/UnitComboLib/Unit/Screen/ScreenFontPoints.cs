namespace UnitComboLib.Unit.Screen
{
  using System;

  public class ScreenFontPoints
  {
    private double mValue = 0;

    #region constructor
    public ScreenFontPoints(double value)
    {
      this.mValue = value;
    }

    private ScreenFontPoints()
    {      
    }
    #endregion constructor

    #region methods
    public static double ToUnit(double inputValue, Itemkey targetUnit)
    {
      ScreenFontPoints d = new ScreenFontPoints(inputValue);

      return d.ToUnit(targetUnit);
    }

    public double ToUnit(Itemkey targetUnit)
    {
      switch (targetUnit)
      {
        case Itemkey.ScreenPercent:
          return this.mValue * (100 / ScreenConverter.OneHundretPercentFont);

        case Itemkey.ScreenFontPoints:
          return this.mValue;

        default:
          throw new NotImplementedException(targetUnit.ToString());
      }
    }
    #endregion methods
  }
}
