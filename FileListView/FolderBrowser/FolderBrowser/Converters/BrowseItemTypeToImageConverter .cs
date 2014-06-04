namespace FolderBrowser.Converters
{
  using System;
  using System.Windows;
  using System.Windows.Data;
  using System.Windows.Markup;
  using System.Windows.Media;
  using FolderBrowser.ViewModels;

  /// <summary>
  /// XAML markup extension to convert <seealso cref="BrowseItemType"/> enum members
  /// into <seealso cref="ImageSource"/> from ResourceDictionary or fallback from static resource.
  /// </summary>
  [ValueConversion(typeof(BrowseItemType), typeof(ImageSource))]
  [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
  public class BrowseItemTypeToImageConverter : MarkupExtension, IMultiValueConverter
  {
    #region fields
    private static BrowseItemTypeToImageConverter converter;
    #endregion fields

    #region constructor
    /// <summary>
    /// Class constructor
    /// </summary>
    public BrowseItemTypeToImageConverter()
    {
    }
    #endregion constructor

    #region methods
    /// <summary>
    /// Returns an object that is provided
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
        converter = new BrowseItemTypeToImageConverter();
      }

      return converter;
    }

    /// <summary>
    /// Converts a <seealso cref="BrowseItemType"/> enumeration member
    /// into a dynamic resource or a fallback image Url (if dynamic resource is not available).
    /// </summary>
    /// <param name="values"></param>
    /// <param name="targetType"></param>
    /// <param name="parameter"></param>
    /// <param name="culture"></param>
    /// <returns></returns>
    public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
      if (values == null)
        return Binding.DoNothing;

      if (values.Length != 2)
        return Binding.DoNothing;

      if (values.Length != 2)
        return Binding.DoNothing;

      bool? bIsExpanded = values[0] as bool?;
      BrowseItemType? ItemType = values[1] as BrowseItemType?;

      if (bIsExpanded == null && ItemType == null)
      {
        bIsExpanded = values[0] as bool?;
        ItemType = values[1] as BrowseItemType?;

        if (bIsExpanded == null && ItemType == null)
          return Binding.DoNothing;
      }

      if (bIsExpanded == true)
        return GetExpandedImages((BrowseItemType)ItemType);
      else
        return GetNotExpandedImages((BrowseItemType)ItemType);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
    {
      throw new NotImplementedException();
    }

    /// <summary>
    /// Get a DynamicResource from ResourceDictionary or a static ImageSource (as fallback) for not expanded folder item.
    /// </summary>
    /// <param name="ItemType"></param>
    /// <returns></returns>
    private object GetNotExpandedImages(BrowseItemType ItemType)
    {
      string uriPath = null;

      switch (ItemType)
      {
        case BrowseItemType.Folder:
          uriPath = string.Format("FolderItem_Image_{0}", "FolderClosed");
          break;
        case BrowseItemType.HardDisk:
          uriPath = string.Format("FolderItem_Image_{0}", "HardDisk");
          break;
      }

      object item = null;

      if (uriPath != null)
      {
        item = Application.Current.Resources[uriPath];

        if (item != null)
          return item;
      }

      string pathValue = null;

      switch (ItemType)
      {
        case BrowseItemType.Folder:
          pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/FolderClosed.png";
          break;
        case BrowseItemType.HardDisk:
          pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/HardDisk.ico";
          break;
      }

      if (pathValue != null)
      {
        try
        {
          Uri imagePath = new Uri(pathValue, UriKind.RelativeOrAbsolute);
          ImageSource source = new System.Windows.Media.Imaging.BitmapImage(imagePath);

          return source;
        }
        catch
        {
        }
      }

      // Attempt to load fallback folder from ResourceDictionary
      item = Application.Current.Resources[string.Format("FolderItem_Image_{0}", "FolderClosed")];

      if (item != null)
        return item;
      else
      {
        // Attempt to load fallback folder from fixed Uri
        pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/FolderClosed.png";

        try
        {
          Uri imagePath = new Uri(pathValue, UriKind.RelativeOrAbsolute);
          ImageSource source = new System.Windows.Media.Imaging.BitmapImage(imagePath);

          return source;
        }
        catch
        {
        }
      }

      return null;
    }

    /// <summary>
    /// Get a DynamicResource from ResourceDictionary or a static ImageSource (as fallback) for expanded folder item.
    /// </summary>
    /// <param name="ItemType"></param>
    /// <returns></returns>
    private object GetExpandedImages(BrowseItemType ItemType)
    {
      string uriPath = null;

      switch (ItemType)
      {
        case BrowseItemType.Folder:
          uriPath = string.Format("FolderItem_Image_{0}", "FolderOpen");
          break;
        case BrowseItemType.HardDisk:
          uriPath = string.Format("FolderItem_Image_{0}", "HardDisk");
          break;
      }

      object item = null;

      if (uriPath != null)
      {
        item = Application.Current.Resources[uriPath];

        if (item != null)
          return item;
      }

      string pathValue = null;

      switch (ItemType)
      {
        case BrowseItemType.Folder:
          pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/FolderOpen.png";
          break;
        case BrowseItemType.HardDisk:
          pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/HardDisk.ico";
          break;
      }

      if (pathValue != null)
      {
        try
        {
          Uri imagePath = new Uri(pathValue, UriKind.RelativeOrAbsolute);
          ImageSource source = new System.Windows.Media.Imaging.BitmapImage(imagePath);

          return source;
        }
        catch
        {
        }
      }

      // Attempt to load fallback folder from ResourceDictionary
      item = Application.Current.Resources[string.Format("FolderItem_Image_{0}", "FolderOpen")];

      if (item != null)
        return item;
      else
      {
        // Attempt to load fallback folder from fixed Uri
        pathValue = "pack://application:,,,/FolderBrowser;component/Images/Generic/FolderOpen.png";

        try
        {
          Uri imagePath = new Uri(pathValue, UriKind.RelativeOrAbsolute);
          ImageSource source = new System.Windows.Media.Imaging.BitmapImage(imagePath);

          return source;
        }
        catch
        {
        }
      }

      return null;
    }
    #endregion methods
  }
}
