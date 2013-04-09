namespace EdiViews.Converter
{
  using System;
  using System.Globalization;
  using System.Windows.Data;
  using System.Windows.Markup;
  using EdiViews.Config.ViewModel;

  [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
  public class HlThemeKeyToBooleanConverter : MarkupExtension, IMultiValueConverter
  {
    #region field
    private static HlThemeKeyToBooleanConverter converter;
    #endregion field

    #region constructor
    /// <summary>
    /// Standard Constructor
    /// </summary>
    public HlThemeKeyToBooleanConverter()
    {
    }
    #endregion constructor

    #region MarkupExtension
    /// <summary>
    /// When implemented in a derived class, returns an object that is provided
    /// as the value of the target property for this markup extension.
    /// 
    /// When a XAML processor processes a type node and member value that is a markup extension,
    /// it invokes the ProvideValue method of that markup extension and writes the result into the
    /// object graph or serialization stream. The XAML object writer passes service context to each
    /// such implementation through the serviceProvider parameter.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <returns></returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (converter == null)
      {
        converter = new HlThemeKeyToBooleanConverter();
      }

      return converter;
    }
    #endregion MarkupExtension

    #region IValueConverter
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
      if (values == null) return Binding.DoNothing;

      if (values.Length != 2)
        return Binding.DoNothing;

      HlThemeKey checkValue;
      HlThemeKey targetValue;

      try
      {
        checkValue = values[0] as HlThemeKey;
        targetValue = values[1] as HlThemeKey;
      }
      catch
      {
        return Binding.DoNothing;
      }

      if (checkValue == null && targetValue == null)
        return Binding.DoNothing;

      if (checkValue == null && targetValue != null ||
          checkValue != null && targetValue == null)
      {
        return false; 
      }

      bool bRet = checkValue.Equals(targetValue);

      return bRet;
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
    #endregion IValueConverter
  }
}
