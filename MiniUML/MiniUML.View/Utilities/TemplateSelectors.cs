﻿namespace MiniUML.View.Utilities
{
  using System.Windows;
  using System.Windows.Controls;
  using System.Xml.Linq;
  using MiniUML.Framework;

  public class ShapeTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if (item != null && item is XElement)
      {
        XElement element = item as XElement;

        DataTemplate template = PluginManager.PluginResources[element.Name.LocalName] as DataTemplate;

        if (template != null)
          return template;
        else
          return PluginManager.PluginResources["MiniUML.UnknownShape"] as DataTemplate;
      }

      return null;
    }
  }

  public class ArrowTemplateSelector : DataTemplateSelector
  {
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
      if (item == null)
        return null;

      DataTemplate d = PluginManager.PluginResources[item.ToString()] as DataTemplate;

      return d;
    }
  }
}
