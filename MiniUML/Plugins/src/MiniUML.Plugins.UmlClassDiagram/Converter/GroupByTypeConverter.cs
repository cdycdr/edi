namespace MiniUML.Plugins.UmlClassDiagram.Converter
{
  using System;
  using System.Globalization;
  using System.Windows.Data;

  public class GroupByTypeConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      switch (value as string)
      {
        case "Uml.Members.Method":
          return "Methods";

        case "Uml.Members.Property":
          return "Properties";

        case "Uml.Members.Field":
          return "Fields";

        case "Uml.Members.Event":
          return "Events";

        case "Uml.Members.Member":
          return "Members";

        case "Uml.Members.Type":
          return "Types";

        default: return value.ToString();
      }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}
