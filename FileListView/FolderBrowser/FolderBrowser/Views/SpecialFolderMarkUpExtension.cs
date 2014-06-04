﻿namespace FolderBrowser.Views
{
  using System;
  using System.Reflection;
  using System.Windows;
  using System.Windows.Markup;

  /// <summary>
  /// This MarkupExtension can be used in XAML to Supply a System.Environment.SpecialFolder
  /// enumeration member as CommandParameter or any other binding item.
  /// 
  /// XAML Example: CommandParameter="{views:SpecialFolderMarkUpExtension SpecialFolder=ProgramFiles}"
  /// 
  /// Based on Blog: http://10rem.net/blog/2011/03/09/creating-a-custom-markup-extension-in-wpf-and-soon-silverlight
  /// </summary>
  [MarkupExtensionReturnType(typeof(object))]
  public class SpecialFolderMarkUpExtension : MarkupExtension
  {
    #region constructor
    public SpecialFolderMarkUpExtension()
    {
    }
    #endregion constructor

    #region properties
    [ConstructorArgument("SpecialFolder")]
    public object SpecialFolder { get; set; }
    #endregion properties

    #region methods
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (SpecialFolder is string)
      {
        if (string.IsNullOrEmpty(SpecialFolder as string) == true)
          return null;

        System.Environment.SpecialFolder ret;

        try
        {
          // Attempt conversion of string parameter to enum
          ret = (System.Environment.SpecialFolder)Enum.Parse(typeof(System.Environment.SpecialFolder),
                                                             SpecialFolder as string, true);
        }
        catch
        {
          return null;
        }

        return ret;
      }

      return null;
    }
    #endregion methods
  }
}
