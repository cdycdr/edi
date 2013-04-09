namespace Edi.Converter
{
  using System;
  using System.Windows.Data;
  using Edi.ViewModel;

  /// <summary>
  /// This converter is invoked when a new ActiveDocument (ActiveContent) is being selected and breught into view.
  /// Return the corresponding ViewModel or Binding.DoNothing (if the document type should not be selected and brought
  /// into view via ActiveDocument property in <seealso cref="Workspace"/>.
  /// </summary>
  class ActiveDocumentConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is EdiViews.ViewModel.Base.FileBaseViewModel)
        return value;

      return Binding.DoNothing;
    }

    public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (value is EdiViews.ViewModel.Base.FileBaseViewModel)
        return value;

      return Binding.DoNothing;
    }
  }
}
