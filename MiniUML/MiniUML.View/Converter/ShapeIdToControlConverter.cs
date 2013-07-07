﻿namespace MiniUML.View.Converter
{
  using System;
  using System.Globalization;
  using System.Windows;
  using System.Windows.Data;

  using MiniUML.Framework;
  using MiniUML.View.Controls;
  using MiniUML.View.Views;

  /// <summary>
  /// Converts a shape id into a shape control.
  /// </summary>
  public class ShapeIdToControlConverter : DependencyObject, IValueConverter
  {
    #region fields
    private static readonly DependencyProperty ReferenceControlProperty =
                            DependencyProperty.Register("ReferenceControl",
                                                        typeof(UIElement),
                                                        typeof(ShapeIdToControlConverter),
                                                        new FrameworkPropertyMetadata(null));
    #endregion fields

    #region properties
    public UIElement ReferenceControl
    {
      get { return (UIElement)GetValue(ReferenceControlProperty); }
      set { SetValue(ReferenceControlProperty, value); }
    }
    #endregion properties

    #region methods
    /// <summary>
    /// Convert shapeID of type <seealso cref="string"/> of a CanvasView control
    /// into a reference to a CanvasView <seealso cref="FrameworkElement"/>,
    /// The output <seealso cref="FrameworkElement"/> is located
    /// in the vicinity (below) the input control (AnchorPoint) represented by the shapeID.
    /// </summary>
    /// <param name="shapeID"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object shapeID, Type targetType, object parameter, CultureInfo culture)
    {
      object o = convert(shapeID, targetType, parameter, culture);

      if (o == null)
        o = AnchorPoint.InvalidSnapTarget;

      return o;
    }

    private object convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      String shape = value as String;

      // Convert invalid shapeID into invalid snap target
      if (shape == null || shape == "" || ReferenceControl == null)
        return null;

      // Find canvas view through reference on this converter
      CanvasView cv = CanvasView.GetCanvasView(ReferenceControl);

      // No canvas view found
      if (cv == null)
        return null;

      // Use CanvasView to find canvas object in vicinity
      return cv.ControlFromElement(cv._CanvasViewModel._DocumentViewModel.dm_DocumentDataModel.GetShapeById(shape));
    }

    /// <summary>
    /// Convert given <seealso cref="UIElement"/> reference from <seealso cref="CanvasView"/>
    /// into shapeID of type <seealso cref="string"/> (equivalent to Id attribute in XElement).
    /// </summary>
    /// <param name="value"></param>
    /// <param name="type"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object ConvertBack(object value, Type type, object parameter, CultureInfo culture)
    {
      UIElement control = value as UIElement;

      if (control == null)
        return "";

      CanvasView cv = CanvasView.GetCanvasView(control);

      return cv.ElementFromControl(control).GetStringAttribute("Id");
    }
    #endregion methods
  }
}
